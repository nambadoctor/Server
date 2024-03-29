﻿using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface INoteService
    {
        public Task<List<ProviderClientOutgoing.NoteOutgoing>> GetAppointmentNotes(string ServiceRequestId);
        public Task SetNote(ProviderClientIncoming.NoteIncoming noteIncoming);
        public Task UpdateNote(ProviderClientIncoming.NoteIncoming noteIncoming);
        public Task SetStrayNote(ProviderClientIncoming.NoteIncoming noteIncoming, string AppointmentId, string ServiceRequestId);
        public Task DeleteNote(string ReportId);
        public Task<List<ProviderClientOutgoing.NoteOutgoing>> GetAllNotes(string organisationId, string customerId);
    }
}