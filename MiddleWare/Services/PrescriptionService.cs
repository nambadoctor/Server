using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

using MiddleWare.Interfaces;
using DataLayer;
using ND.DataLayer.Utils.BlobStorage;
using MongoDB.Bson;
using DataModel.Shared;
using Exceptions = DataModel.Shared.Exceptions;
using MiddleWare.Utils;
using MongoDB.GenericRepository.Interfaces;
using MiddleWare.Converters;

namespace MiddleWare.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private IPrescriptionRepository prescriptionRepository;
        private IAppointmentRepository appointmentRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IAppointmentRepository appointmentRepository, IServiceRequestRepository serviceRequestRepository, IMediaContainer mediaContainer, ILogger<PrescriptionService> logger)
        {
            this.prescriptionRepository = prescriptionRepository;
            this.appointmentRepository = appointmentRepository;
            this.serviceRequestRepository = serviceRequestRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }
        public async Task DeletePrescriptionDocument(string PrescriptionDocumentId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:DeletePrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(PrescriptionDocumentId, IdType.Prescription);

                await prescriptionRepository.DeletePrescriptionDocument(PrescriptionDocumentId);
            }

        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAllPrescriptions(string organisationId, string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:GetAllPrescriptions"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);
                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var prescriptionDocuments = await prescriptionRepository.GetAllPrescriptions(organisationId, customerId);

                if (prescriptionDocuments == null)
                {
                    logger.LogInformation("No prescriptions available");
                    return new List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>();
                }

                return await GetOutgoingPrescriptionDocumentsWithSasUrl(prescriptionDocuments);
            }
        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:GetAppointmentPrescriptions"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var prescriptionDocuments = await prescriptionRepository.GetServiceRequestPrescriptionDocuments(ServiceRequestId);

                if (prescriptionDocuments == null)
                {
                    logger.LogInformation("No prescriptions available");
                    return new List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>();
                }

                return await GetOutgoingPrescriptionDocumentsWithSasUrl(prescriptionDocuments);
            }

        }

        public async Task SetPrescriptionDocument(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:SetPrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(prescriptionDocumentIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(prescriptionDocumentIncoming.AppointmentId, IdType.Appointment);

                var prescriptionDocument = ServiceRequestConverter.ConvertToMongoPrescriptionDocument(prescriptionDocumentIncoming);
                //Upload to blob
                var uploaded = await mediaContainer.UploadFileToStorage(ByteHandler.Base64DecodeFileString(prescriptionDocumentIncoming.File), prescriptionDocument.FileInfo.FileInfoId.ToString());

                await prescriptionRepository.AddPrescriptionDocument(prescriptionDocument, prescriptionDocumentIncoming.ServiceRequestId);
            }

        }

        private async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetOutgoingPrescriptionDocumentsWithSasUrl(List<Mongo.PrescriptionDocument> prescriptionDocuments)
        {
            var listToReturn = new List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>();

            //Generate sas for each file

            foreach (var prescDocument in prescriptionDocuments)
            {
                var sasUrl = await mediaContainer.GetSasUrl(prescDocument.FileInfo.FileInfoId.ToString());

                if (sasUrl != null)
                {
                    listToReturn.Add(
                        ServiceRequestConverter.ConvertToClientOutgoingPrescriptionDocument(prescDocument, sasUrl)
                    );
                }
                else
                {
                    throw new Exceptions.BlobStorageException($"Prescription document not found in blob:{prescDocument.PrescriptionDocumentId}");
                }

            }

            return listToReturn;
        }

    }
}
