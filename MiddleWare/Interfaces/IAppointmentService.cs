﻿using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Mongo;

namespace MiddleWare.Interfaces
{
    public interface IAppointmentService
    {
        public Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string serviceProviderId, string appointmentId);
        public Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetAppointments(string organsiationId, List<string> serviceProviderIds, long startDateTimestamp, long endDateTimestamp);
        public Task<Appointment> UpsertAppointmentForStrayDocuments(string OrganisationId, string ServiceProviderId, string CustomerId);
        public Task AddAppointment(ProviderClientIncoming.AppointmentIncoming appointment);
        public Task CancelAppointment(ProviderClientIncoming.AppointmentIncoming appointment);
        public Task RescheduleAppointment(ProviderClientIncoming.AppointmentIncoming appointment);
        public Task EndAppointment(ProviderClientIncoming.AppointmentIncoming appointment);
    }
}
