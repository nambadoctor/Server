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
    public class ReportService : IReportService
    {
        private IServiceRequestRepository serviceRequestRepository;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public ReportService(IServiceRequestRepository serviceRequestRepository, IMediaContainer mediaContainer, ILogger<ReportService> logger)
        {
            this.serviceRequestRepository = serviceRequestRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }

        public async Task DeleteReport(string ServiceRequestId, string ReportId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:DeleteReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ReportId, IdType.Report);
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var serviceRequest = await serviceRequestRepository.GetServiceRequest(ServiceRequestId);

                DataValidation.ValidateObject(serviceRequest);

                if (serviceRequest.Reports == null)
                {
                    throw new Exceptions.ResourceNotFoundException($"Report documents not found for ServiceRequest id :{ServiceRequestId}");
                }

                var indexOfDocumentToDelete = serviceRequest.Reports.FindIndex(report => report.ReportId == new ObjectId(ReportId));

                if (indexOfDocumentToDelete == -1)
                {
                    throw new Exceptions.ResourceNotFoundException($"Report document with id {ReportId} not found in report list");
                }
                else
                {
                    serviceRequest.Reports.RemoveAt(indexOfDocumentToDelete);
                }

                logger.LogInformation("Setting service request with deleted report metadata");

                await serviceRequestRepository.UpdateServiceRequest(serviceRequest);
            }

        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:GetAppointmentReports"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var serviceRequest = await serviceRequestRepository.GetServiceRequest(ServiceRequestId);

                DataValidation.ValidateObject(serviceRequest);

                var listToReturn = new List<ProviderClientOutgoing.ReportOutgoing>();

                if (serviceRequest.Reports != null)
                {
                    foreach (var report in serviceRequest.Reports)
                    {
                        var sasUrl = await mediaContainer.GetSasUrl(report.ReportId.ToString());

                        if (sasUrl != null)
                        {
                            listToReturn.Add(
                                ServiceRequestConverter.ConvertToClientOutgoingReport(report, sasUrl)
                            );
                        }
                        else
                        {
                            throw new Exceptions.BlobStorageException($"Report not found in blob:{report.ReportId}");
                        }

                    }
                }

                return listToReturn;
            }

        }

        public async Task SetReport(ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:SetReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(reportIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(reportIncoming.AppointmentId, IdType.Appointment);

                var serviceRequestFromDb = await serviceRequestRepository.GetServiceRequest(reportIncoming.AppointmentId);

                DataValidation.ValidateObject(serviceRequestFromDb);

                //Construct new service request to write
                var serviceRequest = new Mongo.ServiceRequest();
                serviceRequest.CustomerId = serviceRequestFromDb.CustomerId;
                serviceRequest.ServiceRequestId = new ObjectId(reportIncoming.ServiceRequestId);
                serviceRequest.Reports = new List<Mongo.Report>();

                if (serviceRequestFromDb.Reports != null)
                {
                    serviceRequest.Reports.AddRange(serviceRequestFromDb.Reports);
                }

                logger.LogInformation($"Begin data conversion ConvertToMongoReport");

                //Add new prescription document to list
                var report = ServiceRequestConverter.ConvertToMongoReport(reportIncoming);
                serviceRequest.Reports.Add(report);

                logger.LogInformation($"Finished data conversion ConvertToMongoReport");

                //Upload to blob
                var uploaded = await mediaContainer.UploadFileToStorage(ByteHandler.Base64Decode(reportIncoming.File), report.ReportId.ToString());

                if (uploaded == null)
                {
                    throw new IOException("Unable to write file to blob storage");
                }

                logger.LogInformation("Begin setting service request with report documents");

                await serviceRequestRepository.UpdateServiceRequest(serviceRequest);

                logger.LogInformation("Finished setting service request with report documents");
            }

        }

    }
}
