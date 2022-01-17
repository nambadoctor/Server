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
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IMediaContainer mediaContainer, ILogger<PrescriptionService> logger)
        {
            this.prescriptionRepository = prescriptionRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }
        public async Task DeletePrescriptionDocument(string ServiceRequestId, string PrescriptionDocumentId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:DeletePrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(PrescriptionDocumentId, IdType.Prescription);
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                await prescriptionRepository.DeletePrescriptionDocument(ServiceRequestId, PrescriptionDocumentId);
            }

        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:GetAppointmentPrescriptions"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var prescriptionDocuments = await prescriptionRepository.GetServiceRequestPrescriptionDocuments(ServiceRequestId);

                DataValidation.ValidateObject(prescriptionDocuments);

                var listToReturn = new List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>();

                //Generate sas for each file

                foreach (var prescDocument in prescriptionDocuments)
                {
                    var sasUrl = await mediaContainer.GetSasUrl(prescDocument.PrescriptionDocumentId.ToString());

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

        public async Task SetPrescriptionDocument(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:SetPrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(prescriptionDocumentIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(prescriptionDocumentIncoming.AppointmentId, IdType.Appointment);

                var prescriptionDocument = ServiceRequestConverter.ConvertToMongoPrescriptionDocument(prescriptionDocumentIncoming);

                string[] splitFileString = prescriptionDocumentIncoming.File.Split(",");
                byte[] decodedPrescription = Convert.FromBase64String(splitFileString.Last());
                //Upload to blob
                var uploaded = await mediaContainer.UploadFileToStorage(decodedPrescription, prescriptionDocument.PrescriptionDocumentId.ToString());

                await prescriptionRepository.AddPrescriptionDocument(prescriptionDocument, prescriptionDocumentIncoming.ServiceRequestId);
            }

        }

    }
}
