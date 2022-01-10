using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

using MiddleWare.Interfaces;
using DataLayer;
using ND.DataLayer.Utils.BlobStorage;
using MongoDB.Bson;
using DataModel.Shared;
using DataModel.Shared.Exceptions;
using MiddleWare.Utils;

namespace MiddleWare.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private IMongoDbDataLayer datalayer;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public PrescriptionService(IMongoDbDataLayer dataLayer, IMediaContainer mediaContainer, ILogger<PrescriptionService> logger)
        {
            this.datalayer = dataLayer;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }
        public async Task<string> DeletePrescriptionDocument(string CustomerId, string AppointmentId, string PrescriptionDocumentId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:DeletePrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(PrescriptionDocumentId) || !ObjectId.TryParse(PrescriptionDocumentId, out ObjectId prescriptionDocumentId))
                    {
                        throw new ArgumentException("Prescription Document Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new ArgumentException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(AppointmentId) || !ObjectId.TryParse(AppointmentId, out ObjectId appointmentId))
                    {
                        throw new ArgumentException("Appointment Id is invalid");
                    }

                    var serviceRequest = await datalayer.GetServiceRequest(AppointmentId);

                    if (serviceRequest == null)
                    {
                        throw new ServiceRequestDoesNotExistException($"Service request not found for appointment id :{AppointmentId}");
                    }

                    if (serviceRequest.PrescriptionDocuments == null)
                    {
                        throw new PrescriptionDoesNotExistException($"Prescription documents not found for appointment id :{AppointmentId}");
                    }

                    var indexOfDocumentToDelete = serviceRequest.PrescriptionDocuments.FindIndex(document => document.PrescriptionDocumentId == prescriptionDocumentId);

                    if (indexOfDocumentToDelete == -1)
                    {
                        throw new PrescriptionDoesNotExistException($"Prescription document with id {PrescriptionDocumentId} not found in list of docs");
                    }
                    else
                    {
                        serviceRequest.PrescriptionDocuments.RemoveAt(indexOfDocumentToDelete);
                    }

                    logger.LogInformation("Setting service request with deleted prescription metadata");

                    await datalayer.SetServiceRequest(serviceRequest);

                    return PrescriptionDocumentId;
                }
                finally
                {

                }
            }

        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string CustomerId, string AppointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:GetAppointmentPrescriptions"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new ArgumentException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(AppointmentId) || !ObjectId.TryParse(AppointmentId, out ObjectId appointmentId))
                    {
                        throw new ArgumentException("Appointment Id is invalid");
                    }

                    var serviceRequest = await datalayer.GetServiceRequest(AppointmentId);

                    if (serviceRequest == null)
                    {
                        throw new ServiceRequestDoesNotExistException($"Service request not found for appointment id :{AppointmentId}");
                    }

                    var listToReturn = new List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>();

                    if (serviceRequest.PrescriptionDocuments != null)
                    {
                        foreach (var prescDocument in serviceRequest.PrescriptionDocuments)
                        {
                            var sasUrl = await mediaContainer.DownloadFileFromStorage(prescDocument.PrescriptionDocumentId.ToString());

                            if (sasUrl != null)
                            {
                                listToReturn.Add(
                                    ConvertToClientOutgoingPrescriptionDocument(prescDocument, sasUrl)
                                );
                            }
                            else
                            {
                                throw new PrescriptionDoesNotExistException($"Prescription document not found in blob:{prescDocument.PrescriptionDocumentId}");
                            }

                        }
                    }

                    return listToReturn;
                }
                finally
                {

                }
            }

        }

        public async Task<ProviderClientOutgoing.PrescriptionDocumentOutgoing> SetPrescriptionDocument(string CustomerId, ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:SetPrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(prescriptionDocumentIncoming.ServiceRequestId) || !ObjectId.TryParse(prescriptionDocumentIncoming.ServiceRequestId, out ObjectId serviceRequestId))
                    {
                        throw new ArgumentException("Service request Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new ArgumentException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(prescriptionDocumentIncoming.AppointmentId) || !ObjectId.TryParse(prescriptionDocumentIncoming.AppointmentId, out ObjectId appointmentId))
                    {
                        throw new ArgumentException("Appointment Id is invalid");
                    }

                    var serviceRequestFromDb = await datalayer.GetServiceRequest(prescriptionDocumentIncoming.AppointmentId);

                    if (serviceRequestFromDb == null)
                    {
                        throw new ServiceRequestDoesNotExistException($"Service request does not exist for appointment: {prescriptionDocumentIncoming.AppointmentId}");
                    }

                    //Construct new service request to write
                    var serviceRequest = new Mongo.ServiceRequest();
                    serviceRequest.CustomerId = CustomerId;
                    serviceRequest.ServiceRequestId = serviceRequestId;
                    serviceRequest.PrescriptionDocuments = new List<Mongo.PrescriptionDocument>();

                    if (serviceRequestFromDb.PrescriptionDocuments != null)
                    {
                        serviceRequest.PrescriptionDocuments.AddRange(serviceRequestFromDb.PrescriptionDocuments);
                    }

                    logger.LogInformation($"Begin data conversion ConvertToMongoPrescriptionDocument");

                    //Add new prescription document to list
                    var prescriptionDocument = ConvertToMongoPrescriptionDocument(prescriptionDocumentIncoming);
                    serviceRequest.PrescriptionDocuments.Add(prescriptionDocument);

                    logger.LogInformation($"Finished data conversion ConvertToMongoPrescriptionDocument");

                    //Upload to blob
                    var uploaded = await mediaContainer.UploadFileToStorage(ByteHandler.Base64Decode(prescriptionDocumentIncoming.File), prescriptionDocument.PrescriptionDocumentId.ToString());

                    if (uploaded == null)
                    {
                        throw new IOException("Unable to write file to blob storage");
                    }

                    logger.LogInformation("Begin setting service request with prescription documents");

                    var response = await datalayer.SetServiceRequest(serviceRequest);

                    logger.LogInformation("Finished setting service request with prescription documents");

                    //Construct outgoing prescription document which has sas url
                    var sasUrl = await mediaContainer.DownloadFileFromStorage(prescriptionDocument.PrescriptionDocumentId.ToString());

                    if (sasUrl == null)
                    {
                        throw new PrescriptionDoesNotExistException($"Error generating sas url for id : {prescriptionDocument.PrescriptionDocumentId}");
                    }

                    logger.LogInformation($"Begin data conversion ConvertToClientOutgoingPrescriptionDocument");

                    var outgoingPrescriptionDocument = ConvertToClientOutgoingPrescriptionDocument(prescriptionDocument, sasUrl);

                    logger.LogInformation($"Finished data conversion ConvertToClientOutgoingPrescriptionDocument");

                    return outgoingPrescriptionDocument;
                }
                finally
                {

                }
            }

        }

        private Mongo.PrescriptionDocument ConvertToMongoPrescriptionDocument(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            var mongoPrescriptionDocument = new Mongo.PrescriptionDocument();

            mongoPrescriptionDocument.PrescriptionDocumentId = ObjectId.GenerateNewId();

            var fileInfo = new Mongo.FileInfo();
            {
                fileInfo.FileInfoId = ObjectId.GenerateNewId();
                fileInfo.FileName = prescriptionDocumentIncoming.FileName;
                fileInfo.FileType = prescriptionDocumentIncoming.FileType;
            };

            mongoPrescriptionDocument.FileInfo = fileInfo;

            var detail = new Mongo.PrescriptionDetail();
            detail.Name = prescriptionDocumentIncoming.Details;
            detail.Type = prescriptionDocumentIncoming.DetailsType;
            mongoPrescriptionDocument.PrescriptionDetail = detail;

            return mongoPrescriptionDocument;
        }

        private ProviderClientOutgoing.PrescriptionDocumentOutgoing ConvertToClientOutgoingPrescriptionDocument(Mongo.PrescriptionDocument mongoPrescriptionDocument, string SasUrl)
        {
            var prescriptionDocumentOutgoing = new ProviderClientOutgoing.PrescriptionDocumentOutgoing();

            prescriptionDocumentOutgoing.PrescriptionDocumentId = mongoPrescriptionDocument.PrescriptionDocumentId.ToString();

            prescriptionDocumentOutgoing.Name = mongoPrescriptionDocument.FileInfo.FileName;
            prescriptionDocumentOutgoing.FileType = mongoPrescriptionDocument.FileInfo.FileType;

            if (mongoPrescriptionDocument.PrescriptionDetail != null)
            {
                prescriptionDocumentOutgoing.Details = mongoPrescriptionDocument.PrescriptionDetail.Details;
                prescriptionDocumentOutgoing.DetailsType = mongoPrescriptionDocument.PrescriptionDetail.Type;
            }

            prescriptionDocumentOutgoing.SasUrl = SasUrl;

            return prescriptionDocumentOutgoing;
        }
    }
}
