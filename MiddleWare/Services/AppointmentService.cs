using DataLayer;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using DataModel.Shared;
using Exceptions = DataModel.Shared.Exceptions;
using MiddleWare.Utils;
using DataModel.Shared.Exceptions;
using MongoDB.GenericRepository.Interfaces;

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
                try
                {

                    DataValidation.ValidateIncomingId(serviceProviderId, IdType.ServiceProvider);

                    DataValidation.ValidateIncomingId(appointmentId, IdType.Appointment);

                    var appointment = await appointmenRepository.GetAppointment(serviceProviderId, appointmentId);

                    DataValidation.ValidateObject(appointment);

                    logger.LogInformation("Beginning data conversion ConvertToClientAppointmentData");

                    var appointmentData = AppointmentConverter.ConvertToClientAppointmentData(appointment);

                    logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                    return appointmentData;
                }
                finally
                {

                }
            }

        }

        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:GetAppointments"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    DataValidation.ValidateIncomingId(organsiationId, IdType.Organisation);

                    var appointments = await appointmenRepository.GetAppointmentsByServiceProvider(organsiationId, serviceProviderIds);
                    //Piece together all the objects
                    logger.LogInformation("Beginning data conversion ConvertToClientAppointmentData");

                    var listToReturn = new List<ProviderClientOutgoing.OutgoingAppointment>();

                    if (!(appointments == null || appointments.Count == 0))
                    {

                        foreach (var appointment in appointments)
                        {
                            listToReturn.Add(AppointmentConverter.ConvertToClientAppointmentData(
                                    appointment)
                                    );
                        }
                    }

                    logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                    return listToReturn;
                }
                finally
                {

                }
            }

        }

        public async Task SetAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:SetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    DataValidation.ValidateIncomingId(appointment.OrganisationId, IdType.Organisation);
                    DataValidation.ValidateIncomingId(appointment.ServiceProviderId, IdType.ServiceProvider);
                    DataValidation.ValidateIncomingId(appointment.CustomerId, IdType.Customer);

                    //Here appointment id is allowed to be null but if not then throw error if invalid id
                    if (!string.IsNullOrWhiteSpace(appointment.AppointmentId) && ObjectId.TryParse(appointment.AppointmentId, out ObjectId appId) == false)
                    {
                        throw new ArgumentException("Appointment Id was invalid");
                    }

                    var spProfile = await serviceProviderRepository.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

                    DataValidation.ValidateObject(spProfile);

                    var customerProfile = await customerRepository.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

                    DataValidation.ValidateObject(customerProfile);


                    //New appointment
                    if (string.IsNullOrWhiteSpace(appointment.AppointmentId))
                    {
                        logger.LogInformation("New appointment set, appointment id was null");

                        var serviceRequest = new Mongo.ServiceRequest();

                        //Service request and appointment both need Ids
                        var appointmentId = ObjectId.GenerateNewId();
                        var serviceRequestId = ObjectId.GenerateNewId();

                        appointment.AppointmentId = appointmentId.ToString();
                        appointment.ServiceRequestId = serviceRequestId.ToString();

                        serviceRequest.ServiceRequestId = serviceRequestId;
                        serviceRequest.AppointmentId = appointmentId.ToString();
                        serviceRequest.CustomerId = appointment.CustomerId;
                        serviceRequest.OrganisationId = appointment.OrganisationId;
                        serviceRequest.ServiceProviderId = appointment.ServiceProviderId;

                        logger.LogInformation("Begin data conversion ConvertToMongoAppointmentData");

                        var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(spProfile, appointment, customerProfile);

                        logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

                        await appointmenRepository.AddAppointment(mongoAppointment);

                        await serviceRequestRepository.AddServiceRequest(serviceRequest);

                    }
                    else
                    {
                        logger.LogInformation($"Update existing appointment, appointment id {appointment.AppointmentId}");

                        logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

                        var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(spProfile, appointment, customerProfile);

                        logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

                        await appointmenRepository.UpdateAppointment(mongoAppointment);
                    }
                }
                finally
                {

                }
            }


        }
    }
}
