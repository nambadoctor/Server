using DataModel.Client.Admin.Outgoing;
using DataModel.Mongo;
using DataModel.Shared;
using MiddleWare.Interfaces;
using MongoDB.GenericRepository.Interfaces;

namespace MiddleWare.Services
{
    public class AdminService : IAdminService
    {
        private IAppointmentRepository appointmentRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private IOrganisationRepository organisationRepository;
        private ILogger logger;
        public AdminService(IAppointmentRepository appointmentRepository, IServiceRequestRepository serviceRequestRepository, IOrganisationRepository organisationRepository, ILogger<AdminService> logger)
        {
            this.appointmentRepository = appointmentRepository;
            this.serviceRequestRepository = serviceRequestRepository;
            this.organisationRepository = organisationRepository;
            this.logger = logger;
        }
        public async Task<List<OutgoingAdminStat>> GetAdminStats()
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:DeleteNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                //Get appointments for current day
                var appointmentsForDay = await appointmentRepository.GetAllAppointments(
                    DateTime.UtcNow.Date,
                    DateTime.UtcNow.Date.AddDays(1).AddTicks(-1)
                );
                
                var validAppointments = appointmentsForDay.Where(app => app.AppointmentType != DataModel.Mongo.AppointmentType.CustomerManagement).ToList();

                logger.LogInformation($"Total appointments count for day {validAppointments.Count}");

                var serviceRequestIds = validAppointments
                    .Select(app => app.ServiceRequestId)
                    .ToList();

                var organisations = await organisationRepository.GetAll();

                var serviceRequests = await serviceRequestRepository.GetServiceRequestsMatchingId(serviceRequestIds);

                logger.LogInformation($"Total service requests count for day {serviceRequests.Count}");

                var stats = GetStatsForAppointments(validAppointments, serviceRequests, organisations.ToList());

                return stats;
            }
        }

        private List<OutgoingAdminStat> GetStatsForAppointments(List<Appointment> appointments, List<ServiceRequest> serviceRequests, List<Organisation> organisations)
        {
            List<OutgoingAdminStat> outgoingAdminStats = new List<OutgoingAdminStat>();

            var serviceProviderIds = appointments.Select(app => app.ServiceProviderId).Distinct();

            foreach (var spId in serviceProviderIds)
            {
                var appointmentsOfServiceprovider = appointments.Where(app => app.ServiceProviderId == spId).ToList();
                var adminStat = new OutgoingAdminStat();
                var referenceAppointment = appointmentsOfServiceprovider.First();

                adminStat.ServiceProviderName = referenceAppointment.ServiceProviderName;

                adminStat.OrganisationName = organisations
                    .Find(org => org.OrganisationId.ToString() == referenceAppointment.OrganisationId)
                    ?.Name;

                adminStat.NoOfAppointments = appointmentsOfServiceprovider.Count;

                adminStat.NoOfDocumentsUploaded = 0;

                foreach (var appointment in appointmentsOfServiceprovider)
                {
                    var associatedServiceRequest = serviceRequests.Find(sr => sr.ServiceRequestId.ToString() == appointment.ServiceRequestId)!;
                    adminStat.NoOfDocumentsUploaded += associatedServiceRequest.Notes.Count + associatedServiceRequest.PrescriptionDocuments.Count + associatedServiceRequest.Reports.Count;
                }
                
                outgoingAdminStats.Add(adminStat);
            }

            return outgoingAdminStats;
        }
    }
}
