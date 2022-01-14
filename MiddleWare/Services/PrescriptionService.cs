﻿using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
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

namespace MiddleWare.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private IServiceRequestRepository serviceRequestRepository;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public PrescriptionService(IServiceRequestRepository serviceRequestRepository, IMediaContainer mediaContainer, ILogger<PrescriptionService> logger)
        {
            this.serviceRequestRepository = serviceRequestRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }
        public async Task DeletePrescriptionDocument(string CustomerId, string ServiceRequestId, string PrescriptionDocumentId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:DeletePrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    DataValidation.ValidateObjectId(PrescriptionDocumentId, IdType.Prescription);
                    DataValidation.ValidateObjectId(CustomerId, IdType.Customer);
                    DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                    var serviceRequest = await serviceRequestRepository.GetServiceRequest(ServiceRequestId);

                    DataValidation.ValidateObject(serviceRequest);

                    if (serviceRequest.PrescriptionDocuments == null)
                    {
                        throw new Exceptions.ResourceNotFoundException($"Prescription documents not found for ServiceRequestId id :{ServiceRequestId}");
                    }

                    var indexOfDocumentToDelete = serviceRequest.PrescriptionDocuments.FindIndex(document => document.PrescriptionDocumentId == new ObjectId(PrescriptionDocumentId));

                    if (indexOfDocumentToDelete == -1)
                    {
                        throw new Exceptions.ResourceNotFoundException($"Prescription document with id {PrescriptionDocumentId} not found in list of docs");
                    }
                    else
                    {
                        serviceRequest.PrescriptionDocuments.RemoveAt(indexOfDocumentToDelete);
                    }

                    logger.LogInformation("Setting service request with deleted prescription metadata");

                    await serviceRequestRepository.UpdateServiceRequest(serviceRequest);
                }
                finally
                {

                }
            }

        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string CustomerId, string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:GetAppointmentPrescriptions"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                    var serviceRequest = await serviceRequestRepository.GetServiceRequest(ServiceRequestId);

                    DataValidation.ValidateObject(serviceRequest);

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
                                throw new Exceptions.BlobStorageException($"Prescription document not found in blob:{prescDocument.PrescriptionDocumentId}");
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

        public async Task SetPrescriptionDocument(string CustomerId, ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "PrescriptionService:SetPrescriptionDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    DataValidation.ValidateObjectId(prescriptionDocumentIncoming.ServiceRequestId, IdType.ServiceRequest);
                    DataValidation.ValidateObjectId(prescriptionDocumentIncoming.AppointmentId, IdType.Appointment);
                    DataValidation.ValidateObjectId(CustomerId, IdType.Customer);

                    var serviceRequestFromDb = await serviceRequestRepository.GetServiceRequest(prescriptionDocumentIncoming.AppointmentId);

                    DataValidation.ValidateObject(serviceRequestFromDb);

                    //Construct new service request to write
                    var serviceRequest = new Mongo.ServiceRequest();
                    serviceRequest.CustomerId = CustomerId;
                    serviceRequest.ServiceRequestId = new ObjectId(prescriptionDocumentIncoming.ServiceRequestId);
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

                    await serviceRequestRepository.UpdateServiceRequest(serviceRequest);

                    logger.LogInformation("Finished setting service request with prescription documents");

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
