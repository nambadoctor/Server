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
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public ReportService(IReportRepository reportRepository, IAppointmentRepository appointmentRepository, IMediaContainer mediaContainer, ILogger<ReportService> logger)
        {
            this.reportRepository = reportRepository;
            this.appointmentRepository = appointmentRepository;
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

                await reportRepository.DeleteReport(ServiceRequestId, ReportId);

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

        public async Task SetStrayReport(ProviderClientIncoming.ReportIncoming reportIncoming, string ServiceProviderId, string CustomerId)
        {
            using (logger.BeginScope("Method: {Method}", "ReportService:SetStrayReport"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                if (!string.IsNullOrEmpty(reportIncoming.AppointmentId))
                {
                    DataValidation.ValidateObjectId(reportIncoming.AppointmentId, IdType.Appointment);
                }

                if (!string.IsNullOrEmpty(reportIncoming.ServiceRequestId))
                {
                    DataValidation.ValidateObjectId(reportIncoming.ServiceRequestId, IdType.ServiceRequest);
                }

                DataValidation.ValidateObjectId(ServiceProviderId, IdType.ServiceProvider);
                DataValidation.ValidateObjectId(CustomerId, IdType.Customer);

                var customerManagementAppointment = await appointmentRepository.GetAppointmentByType(ServiceProviderId, CustomerId, Mongo.AppointmentType.CustomerManagement);

                if (customerManagementAppointment != null)
                {
                    reportIncoming.ServiceRequestId = customerManagementAppointment.ServiceRequestId;
                    reportIncoming.AppointmentId = customerManagementAppointment.AppointmentId.ToString();
                    await SetReport(reportIncoming);
                } else
                {
                    //TODO: Make Appointment and Set Report
                }
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
