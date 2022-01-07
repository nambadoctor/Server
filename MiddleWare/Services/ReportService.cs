using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;

using MiddleWare.Interfaces;
using DataLayer;
using ND.DataLayer.Utils.BlobStorage;
using MongoDB.Bson;
using DataModel.Shared;

namespace MiddleWare.Services
{
    public class ReportService : IReportService
    {
        private IMongoDbDataLayer datalayer;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public ReportService(IMongoDbDataLayer dataLayer, IMediaContainer mediaContainer, ILogger<ReportService> logger)
        {
            this.datalayer = dataLayer;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }

        public async Task<string> DeleteReport(string CustomerId, string AppointmentId, string ReportId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:DeleteReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(ReportId) || !ObjectId.TryParse(ReportId, out ObjectId reportId))
                    {
                        throw new InvalidDataException("Report Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new InvalidDataException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(AppointmentId) || !ObjectId.TryParse(AppointmentId, out ObjectId appointmentId))
                    {
                        throw new InvalidDataException("Appointment Id is invalid");
                    }

                    var serviceRequest = await datalayer.GetServiceRequest(AppointmentId);

                    if (serviceRequest == null)
                    {
                        throw new InvalidDataException($"Service request not found for appointment id :{AppointmentId}");
                    }

                    if (serviceRequest.Reports == null)
                    {
                        throw new InvalidDataException($"Prescription documents not found for appointment id :{AppointmentId}");
                    }

                    var indexOfDocumentToDelete = serviceRequest.Reports.FindIndex(report => report.ReportId == reportId);

                    if (indexOfDocumentToDelete == -1)
                    {
                        throw new InvalidDataException($"Prescription document with id {reportId} not found");
                    }
                    else
                    {
                        serviceRequest.Reports.RemoveAt(indexOfDocumentToDelete);
                    }

                    await datalayer.SetServiceRequest(serviceRequest);

                    return ReportId;
                }
                finally
                {

                }
            }

        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string AppointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:GetAppointmentReports"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new InvalidDataException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(AppointmentId) || !ObjectId.TryParse(AppointmentId, out ObjectId appointmentId))
                    {
                        throw new InvalidDataException("Appointment Id is invalid");
                    }

                    var serviceRequest = await datalayer.GetServiceRequest(AppointmentId);

                    if (serviceRequest == null)
                    {
                        throw new InvalidDataException($"Service request not found for appointment id :{AppointmentId}");
                    }

                    var listToReturn = new List<ProviderClientOutgoing.ReportOutgoing>();

                    if (serviceRequest.Reports != null)
                    {
                        foreach (var report in serviceRequest.Reports)
                        {
                            var sasUrl = await mediaContainer.DownloadFileFromStorage(report.ReportId.ToString());

                            if (sasUrl != null)
                            {
                                listToReturn.Add(
                                    ConvertToClientOutgoingReport(report, sasUrl)
                                );
                            }
                            else
                            {
                                throw new InvalidDataException($"Report not found in blob:{report.ReportId}");
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

        public async Task<ProviderClientOutgoing.ReportOutgoing> SetReport(string CustomerId, ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:SetReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(reportIncoming.ServiceRequestId) || !ObjectId.TryParse(reportIncoming.ServiceRequestId, out ObjectId serviceRequestId))
                    {
                        throw new InvalidDataException("Service request Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(CustomerId) || !ObjectId.TryParse(CustomerId, out ObjectId customerId))
                    {
                        throw new InvalidDataException("Customer Id is invalid");
                    }

                    if (string.IsNullOrWhiteSpace(reportIncoming.AppointmentId) || !ObjectId.TryParse(reportIncoming.AppointmentId, out ObjectId appointmentId))
                    {
                        throw new InvalidDataException("Appointment Id is invalid");
                    }

                    var serviceRequestFromDb = await datalayer.GetServiceRequest(reportIncoming.AppointmentId);

                    if (serviceRequestFromDb == null)
                    {
                        throw new InvalidDataException("Service request does not exist");
                    }

                    //Construct new service request to write
                    var serviceRequest = new Mongo.ServiceRequest();
                    serviceRequest.CustomerId = CustomerId;
                    serviceRequest.ServiceRequestId = serviceRequestId;
                    serviceRequest.Reports = new List<Mongo.Report>();

                    if (serviceRequestFromDb.Reports != null)
                    {
                        serviceRequest.Reports.AddRange(serviceRequestFromDb.Reports);
                    }

                    //Add new prescription document to list
                    var report = ConvertToMongoReport(reportIncoming);
                    serviceRequest.Reports.Add(report);

                    //Upload to blob
                    var uploaded = await mediaContainer.UploadFileToStorage(reportIncoming.File, report.ReportId.ToString());

                    if (uploaded == null)
                    {
                        throw new IOException("Unable to write file to blob storage");
                    }

                    var response = await datalayer.SetServiceRequest(serviceRequest);

                    //Construct outgoing prescription document which has sas url
                    var sasUrl = await mediaContainer.DownloadFileFromStorage(report.ReportId.ToString());
                    var outgoingPrescriptionDocument = ConvertToClientOutgoingReport(report, sasUrl);

                    return outgoingPrescriptionDocument;
                }
                finally
                {

                }
            }

        }

        private Mongo.Report ConvertToMongoReport(ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            var mongoReport = new Mongo.Report();

            mongoReport.ReportId = ObjectId.GenerateNewId();

            var fileInfo = new Mongo.FileInfo();
            {
                fileInfo.FileInfoId = ObjectId.GenerateNewId();
                fileInfo.FileName = reportIncoming.FileName;
                fileInfo.FileType = reportIncoming.FileType;
            };

            mongoReport.FileInfo = fileInfo;

            var detail = new Mongo.ReportDetails();
            detail.Name = reportIncoming.Details;
            detail.Type = reportIncoming.DetailsType;
            mongoReport.Details = detail;

            return mongoReport;
        }

        private ProviderClientOutgoing.ReportOutgoing ConvertToClientOutgoingReport(Mongo.Report mongoReport, string SasUrl)
        {
            var reportOutgoing = new ProviderClientOutgoing.ReportOutgoing();

            reportOutgoing.ReportId = mongoReport.ReportId.ToString();

            reportOutgoing.Name = mongoReport.FileInfo.FileName;
            reportOutgoing.FileType = mongoReport.FileInfo.FileType;

            if (mongoReport.Details != null)
            {
                reportOutgoing.Details = mongoReport.Details.Details;
                reportOutgoing.DetailsType = mongoReport.Details.Type;
            }

            reportOutgoing.SasUrl = SasUrl;

            return reportOutgoing;
        }
    }
}
