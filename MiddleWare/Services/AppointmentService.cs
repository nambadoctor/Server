using DataLayer;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MongoDB.Bson;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using DataModel.Shared;
using Exceptions = DataModel.Shared.Exceptions;

namespace MiddleWare.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IMongoDbDataLayer datalayer;
        private ILogger logger;

        public AppointmentService(IMongoDbDataLayer dataLayer, ILogger<AppointmentService> logger)
        {
            this.datalayer = dataLayer;
            this.logger = logger;
        }
        public async Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string serviceProviderId, string appointmentId)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:GetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(serviceProviderId) || ObjectId.TryParse(serviceProviderId, out ObjectId spId) == false)
                    {
                        throw new ArgumentException("Service provider Id was invalid");
                    }

                    if (string.IsNullOrWhiteSpace(appointmentId) || ObjectId.TryParse(appointmentId, out ObjectId appId) == false)
                    {
                        throw new ArgumentException("Appointment Id was invalid");
                    }

                    var appointment = await datalayer.GetAppointment(serviceProviderId, appointmentId);

                    if (appointment == null)
                    {
                        throw new Exceptions.AppointmentDoesNotExistException($"Appointment with id:{appointmentId} does not exist");
                    }

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
                    if (string.IsNullOrWhiteSpace(organsiationId) || ObjectId.TryParse(organsiationId, out ObjectId orgId) == false)
                    {
                        throw new ArgumentException("Organisation Id was invalid");
                    }

                    var appointments = await datalayer.GetAppointmentsForServiceProvider(organsiationId, serviceProviderIds);
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

        public async Task<ProviderClientOutgoing.OutgoingAppointment> SetAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (logger.BeginScope("Method: {Method}", "AppointmentService:SetAppointment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(appointment.OrganisationId) || ObjectId.TryParse(appointment.OrganisationId, out ObjectId orgId) == false)
                    {
                        throw new ArgumentException("Organisation Id was invalid");
                    }

                    if (string.IsNullOrWhiteSpace(appointment.ServiceProviderId) || ObjectId.TryParse(appointment.ServiceProviderId, out ObjectId spId) == false)
                    {
                        throw new ArgumentException("Service provider Id was invalid");
                    }

                    if (string.IsNullOrWhiteSpace(appointment.CustomerId) || ObjectId.TryParse(appointment.CustomerId, out ObjectId custId) == false)
                    {
                        throw new ArgumentException("Customer Id was invalid");
                    }

                    //Here appointment id is allowed to be null but if not then throw error if invalid id
                    if (!string.IsNullOrWhiteSpace(appointment.AppointmentId) && ObjectId.TryParse(appointment.AppointmentId, out ObjectId appId) == false)
                    {
                        throw new ArgumentException("Appointment Id was invalid");
                    }

                    var spProfile = await datalayer.GetServiceProviderProfile(appointment.ServiceProviderId, appointment.OrganisationId);

                    if (spProfile == null)
                    {
                        throw new ServiceProviderDoesnotExistsException($"Service provider profile with id:{appointment.ServiceProviderId}  OrgId:{appointment.OrganisationId} does not exist");
                    }

                    var customerProfile = await datalayer.GetCustomerProfile(appointment.CustomerId, appointment.OrganisationId);

                    if (customerProfile == null)
                    {
                        throw new Exceptions.CustomerDoesNotExistException($"Customer profile with id:{appointment.CustomerId} OrgId:{appointment.OrganisationId} does not exist");
                    }


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

                        var generatedAppointment = await datalayer.SetAppointmentWithServiceRequest(
                            mongoAppointment,
                            serviceRequest
                            );

                        logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

                        var clientAppointment = AppointmentConverter.ConvertToClientAppointmentData(generatedAppointment);

                        logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                        return clientAppointment;
                    }
                    else
                    {
                        logger.LogInformation($"Update existing appointment, appointment id {appointment.AppointmentId}");

                        logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

                        var mongoAppointment = AppointmentConverter.ConvertToMongoAppointmentData(spProfile, appointment, customerProfile);

                        logger.LogInformation("Finished data conversion ConvertToMongoAppointmentData");

                        var generatedAppointment = await datalayer.SetAppointment(mongoAppointment);

                        logger.LogInformation("Begin data conversion ConvertToClientAppointmentData");

                        var clientAppointment = AppointmentConverter.ConvertToClientAppointmentData(generatedAppointment);

                        logger.LogInformation("Finished data conversion ConvertToClientAppointmentData");

                        return clientAppointment;
                    }
                }
                finally
                {

                }
            }


        }
    }
}
