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
        private IReportRepository reportRepository;
        private IAppointmentRepository appointmentRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public ReportService(IReportRepository reportRepository, IAppointmentRepository appointmentRepository, IServiceRequestRepository serviceRequestRepository, IMediaContainer mediaContainer, ILogger<ReportService> logger)
        {
            this.reportRepository = reportRepository;
            this.appointmentRepository = appointmentRepository;
            this.serviceRequestRepository = serviceRequestRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }

        public async Task DeleteReport(string ReportId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:DeleteReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ReportId, IdType.Report);

                await reportRepository.DeleteReport(ReportId);

                logger.LogInformation($"Successfully deleted report with ID: {ReportId}");

            }
        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAllReports(string organisationId, string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:GetAllReports"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);
                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var reports = await reportRepository.GetAllReports(organisationId, customerId);

                if (reports == null)
                {
                    logger.LogInformation("No reports available");
                    return new List<ProviderClientOutgoing.ReportOutgoing>();
                }

                return await GetOutgoingReportsWithSasUrl(reports);
            }
        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:GetAppointmentReports"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var reports = await reportRepository.GetServiceRequestReports(ServiceRequestId);

                if (reports == null)
                {
                    logger.LogInformation("No reports available");
                    return new List<ProviderClientOutgoing.ReportOutgoing>();
                }

                return await GetOutgoingReportsWithSasUrl(reports);
            }

        }

        public async Task SetReport(ProviderClientIncoming.ReportIncoming reportIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:SetReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(reportIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(reportIncoming.AppointmentId, IdType.Appointment);

                var report = ServiceRequestConverter.ConvertToMongoReport(reportIncoming);

                //Upload to blob
                var uploaded = await mediaContainer.UploadFileToStorage(ByteHandler.Base64DecodeFileString(reportIncoming.File), report.FileInfo.FileInfoId.ToString());

                await reportRepository.AddReport(report, reportIncoming.ServiceRequestId);

            }
        }

        private async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetOutgoingReportsWithSasUrl(List<Mongo.Report> reports)
        {
            var listToReturn = new List<ProviderClientOutgoing.ReportOutgoing>();

            foreach (var report in reports)
            {
                var sasUrl = await mediaContainer.GetSasUrl(report.FileInfo.FileInfoId.ToString());

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

            return listToReturn;
        }

    }
}
