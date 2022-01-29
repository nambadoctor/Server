using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Mongo = DataModel.Mongo;
using MiddleWare.Interfaces;
using DataModel.Shared;
using Exceptions = DataModel.Shared.Exceptions;
using MiddleWare.Utils;
using MongoDB.GenericRepository.Interfaces;
using MiddleWare.Converters;

namespace MiddleWare.Services
{
    public class NoteService : INoteService
    {
        private INoteRepository noteRepository;
        private ILogger logger;

        public NoteService(INoteRepository noteRepository, ILogger<ReportService> logger)
        {
            this.noteRepository = noteRepository;
            this.logger = logger;
        }

        public async Task DeleteNote(string ServiceRequestId, string NoteId)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:DeleteNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(NoteId, IdType.Note);
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                await noteRepository.DeleteNote(ServiceRequestId, NoteId);

                logger.LogInformation($"Deleted note with id:{NoteId}");
            }
        }

        public async Task<List<ProviderClientOutgoing.NoteOutgoing>> GetAllNotes(string organisationId, string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:GetAllNotes"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);
                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var notes = await noteRepository.GetAllNotes(organisationId, customerId);

                if (notes == null)
                {
                    logger.LogInformation("No notes available");
                    return new List<ProviderClientOutgoing.NoteOutgoing>();
                }

                logger.LogInformation($"Received {notes.Count} notes from db");

                var outgoingNotes = ServiceRequestConverter.ConvertToClientOutGoingNotes(notes);

                logger.LogInformation("Converted mongo notes to outgoing notes successfully");

                return outgoingNotes;
            }
        }

        public async Task<List<ProviderClientOutgoing.NoteOutgoing>> GetAppointmentNotes(string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:GetAppointmentNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var notes = await noteRepository.GetServiceRequestNotes(ServiceRequestId);

                if (notes == null)
                {
                    logger.LogInformation("No reports available");
                    return new List<ProviderClientOutgoing.NoteOutgoing>();
                }

                logger.LogInformation($"Received {notes.Count} notes from db");

                var outgoingNotes = ServiceRequestConverter.ConvertToClientOutGoingNotes(notes);

                logger.LogInformation("Converted mongo notes to outgoing notes successfully");

                return outgoingNotes;
            }

        }

        public async Task SetNote(ProviderClientIncoming.NoteIncoming noteIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:SetNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(noteIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(noteIncoming.AppointmentId, IdType.Appointment);

                var mongoNote = ServiceRequestConverter.ConvertToMongoNote(noteIncoming);

                logger.LogInformation("Converted to mongo mote successfully");

                await noteRepository.AddNote(mongoNote, noteIncoming.ServiceRequestId);

                logger.LogInformation($"Added note with id {mongoNote.NoteId} successfully");
            }
        }

        public async Task SetStrayNote(ProviderClientIncoming.NoteIncoming noteIncoming, string AppointmentId, string ServiceRequestId)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:SetStrayNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                if (!string.IsNullOrEmpty(noteIncoming.AppointmentId))
                {
                    DataValidation.ValidateObjectId(noteIncoming.AppointmentId, IdType.Appointment);
                }

                if (!string.IsNullOrEmpty(noteIncoming.ServiceRequestId))
                {
                    DataValidation.ValidateObjectId(noteIncoming.ServiceRequestId, IdType.ServiceRequest);
                }

                noteIncoming.ServiceRequestId = ServiceRequestId;
                noteIncoming.AppointmentId = AppointmentId;
                await SetNote(noteIncoming);
            }
        }
    }
}
