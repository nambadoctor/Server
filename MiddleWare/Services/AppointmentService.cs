
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using MongoDB.GenericRepository.Interfaces;
using Mongo = DataModel.Mongo;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;

namespace MiddleWare.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IAppointmentRepository appointmenRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private ILogger logger;

        public AppointmentService(IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IAppointmentRepository appointmenRepository, IServiceRequestRepository serviceRequestRepository, ILogger<AppointmentService> logger)
        {
            this.logger = logger;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.appointmenRepository = appointmenRepository;
            this.serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:GetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {


                DataValidation.ValidateObjectId(serviceProviderId, IdType.ServiceProvider);

                DataValidation.ValidateObjectId(appointmentId, IdType.Appointment);

                var appointment = await appointmenRepository.GetAppointment(serviceProviderId, appointmentId);

                DataValidation.ValidateObject(appointment);

                var customerProfile = await customerRepository.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

                DataValidation.ValidateObject(customerProfile);

                var serviceRequest = await serviceRequestRepository.GetById(appointment.ServiceRequestId);

                DataValidation.ValidateObject(serviceRequest);

                logger.LogInformation("Beginning data conversion ConvertToClientAppointmentData");

                var appointmentData = GetOutgoingAppointment(appointment, serviceRequest, customerProfile);

                logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                return appointmentData;

            }

        }

        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:GetAppointments"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {

                DataValidation.ValidateObjectId(organsiationId, IdType.Organisation);

                var appointments = await appointmenRepository.GetAppointmentsByServiceProvider(organsiationId, serviceProviderIds);

                appointments.RemoveAll(appointment => appointment.Status == Mongo.AppointmentStatus.Cancelled);

                logger.LogInformation($"Retrieved {appointments.Count} Valid Appointments");

                var serviceRequests = await serviceRequestRepository.GetServiceRequests(
                    appointments.Select(app => app.ServiceRequestId).Distinct().ToList()
                );

                logger.LogInformation($"Retrieved {serviceRequests.Count} Service Requests");

                var customerProfiles = await customerRepository.GetCustomerProfiles(
                    appointments.Select(app => app.CustomerId).Distinct().ToList(),
                    organsiationId
                );

                logger.LogInformation($"Retrieved {customerProfiles.Count} Customer profiles");

                logger.LogInformation("Beginning data conversion ConvertToClientAppointmentData");

                var listToReturn = new List<ProviderClientOutgoing.OutgoingAppointment>();

                foreach (var appointment in appointments)
                {
                    var customerProfile = customerProfiles.Find(cust => cust.CustomerId == appointment.CustomerId);
                    if (customerProfile == null)
                    {
                        logger.LogInformation($"No customer profile found for appointment id: {appointment.AppointmentId}");
                        continue;
                    }
                    var serviceRequest = serviceRequests.Find(sr => sr.ServiceRequestId.ToString() == appointment.ServiceRequestId);
                    if (serviceRequest == null)
                    {
                        logger.LogInformation($"No serviceRequest found for appointment id: {appointment.AppointmentId}");
                        continue;
                    }

                    listToReturn.Add(GetOutgoingAppointment(appointment, serviceRequest, customerProfile));
                }

                logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                return listToReturn;

            }
        }

        public async Task AddAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:AddAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {

                var users = await VerifyAndGetAppointmentUsers(appointment); //Validate customer and service provider

                //Generate new service request
                var serviceRequest = new Mongo.ServiceRequest();
                var appointmentId = ObjectId.GenerateNewId();
                var serviceRequestId = ObjectId.GenerateNewId();

                appointment.AppointmentId = appointmentId.ToString();
                appointment.ServiceRequestId = serviceRequestId.ToString();

                serviceRequest.ServiceRequestId = serviceRequestId;
                serviceRequest.AppointmentId = appointmentId.ToString();
                serviceRequest.CustomerId = appointment.CustomerId;
                serviceRequest.OrganisationId = appointment.OrganisationId;
                serviceRequest.ServiceProviderId = appointment.ServiceProviderId;
                serviceRequest.Reports = new List<Mongo.Report>();
                serviceRequest.PrescriptionDocuments = new List<Mongo.PrescriptionDocument>();
                serviceRequest.Notes = new List<Mongo.Note>();

                logger.LogInformation("Begin data conversion ConvertToMongoAppointmentData");

                var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(users.Item2, appointment, users.Item1);

                logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

                await appointmenRepository.AddAppointment(mongoAppointment);

                logger.LogInformation("Added appointment successfully");

                await serviceRequestRepository.Add(serviceRequest);

                logger.LogInformation("Added serviceRequest successfully");
            }
        }

        public async Task<Mongo.Appointment> UpsertAppointmentForStrayDocuments(string OrganisationId, string ServiceProviderId, string CustomerId)

        {
            DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);
            DataValidation.ValidateObjectId(ServiceProviderId, IdType.ServiceProvider);
            DataValidation.ValidateObjectId(CustomerId, IdType.Customer);

            var appointment = await appointmenRepository.GetAppointmentByType(OrganisationId, ServiceProviderId, CustomerId, Mongo.AppointmentType.CustomerManagement);

            if (appointment != null)
            {
                return appointment;
            }
            else
            {
                var appointmentToCreate = new Mongo.Appointment();

                //Generate new service request
                var serviceRequest = new Mongo.ServiceRequest();
                var appointmentId = ObjectId.GenerateNewId();
                var serviceRequestId = ObjectId.GenerateNewId();

                appointmentToCreate.AppointmentId = appointmentId;
                appointmentToCreate.ServiceRequestId = serviceRequestId.ToString();
                appointmentToCreate.ServiceProviderId = ServiceProviderId;
                appointmentToCreate.CustomerId = CustomerId;
                appointmentToCreate.AppointmentType = Mongo.AppointmentType.CustomerManagement;
                appointmentToCreate.OrganisationId = OrganisationId;

                serviceRequest.ServiceRequestId = serviceRequestId;
                serviceRequest.AppointmentId = appointmentId.ToString();
                serviceRequest.CustomerId = appointmentToCreate.CustomerId;
                serviceRequest.OrganisationId = appointmentToCreate.OrganisationId;
                serviceRequest.ServiceProviderId = appointmentToCreate.ServiceProviderId;
                serviceRequest.Reports = new List<Mongo.Report>();
                serviceRequest.PrescriptionDocuments = new List<Mongo.PrescriptionDocument>();
                serviceRequest.Notes = new List<Mongo.Note>();

                logger.LogInformation("Created new appointment and service request successfully");

                await appointmenRepository.AddAppointment(appointmentToCreate);

                logger.LogInformation("Added appointment successfully");

                await serviceRequestRepository.Add(serviceRequest);

                logger.LogInformation("Added serviceRequest successfully");

                return appointmentToCreate;
            }
        }

        public async Task CancelAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:AddAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(appointment.AppointmentId, IdType.Appointment);

                var users = await VerifyAndGetAppointmentUsers(appointment); //Validate customer and service provider

                logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

                var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(users.Item2, appointment, users.Item1);
                mongoAppointment.Cancellation = new Mongo.Cancellation();
                mongoAppointment.Cancellation.CancellationID = ObjectId.GenerateNewId();
                //Will move to converter once client supports cancellation object

                logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

                await appointmenRepository.CancelAppointment(mongoAppointment);
            }
        }

        public async Task RescheduleAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            DataValidation.ValidateObjectId(appointment.AppointmentId, IdType.Appointment);

            var users = await VerifyAndGetAppointmentUsers(appointment); //Validate customer and service provider

            logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

            var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(users.Item2, appointment, users.Item1);

            logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

            await appointmenRepository.RescheduleAppointment(mongoAppointment);
        }

        public async Task EndAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            DataValidation.ValidateObjectId(appointment.AppointmentId, IdType.Appointment);

            var users = await VerifyAndGetAppointmentUsers(appointment); //Validate customer and service provider

            logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

            var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(users.Item2, appointment, users.Item1);

            logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

            await appointmenRepository.EndAppointment(mongoAppointment);
        }

        private async Task<(Mongo.CustomerProfile, Mongo.ServiceProviderProfile)> VerifyAndGetAppointmentUsers(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            DataValidation.ValidateObjectId(appointment.ServiceProviderId, IdType.ServiceProvider);
            DataValidation.ValidateObjectId(appointment.CustomerId, IdType.Customer);
            DataValidation.ValidateObjectId(appointment.OrganisationId, IdType.Organisation);

            //Here appointment id is allowed to be null but if not then throw error if invalid id
            if (!string.IsNullOrWhiteSpace(appointment.AppointmentId) && ObjectId.TryParse(appointment.AppointmentId, out ObjectId appId) == false)
            {
                throw new ArgumentException("Appointment Id is invalid");
            }

            var spProfile = await serviceProviderRepository.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

            DataValidation.ValidateObject(spProfile);

            var customerProfile = await customerRepository.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

            DataValidation.ValidateObject(customerProfile);

            return (customerProfile, spProfile);
        }

        private ProviderClientOutgoing.OutgoingAppointment GetOutgoingAppointment(
            Mongo.Appointment appointment,
            Mongo.ServiceRequest serviceRequest,
            Mongo.CustomerProfile customerProfile)
        {
            var customerPhoneNumber = customerProfile.PhoneNumbers.First();

            var notesCount = serviceRequest.Notes?.Count ?? 0;

            var reportCount = serviceRequest.Reports?.Count ?? 0;

            var prescriptionCount = serviceRequest.PrescriptionDocuments?.Count ?? 0;

            var appointmentData = AppointmentConverter.ConvertToClientAppointmentData(
                appointment,
                customerPhoneNumber.CountryCode + customerPhoneNumber.Number,
                notesCount,
                reportCount,
                prescriptionCount);

            return appointmentData;
        }

    }
}
