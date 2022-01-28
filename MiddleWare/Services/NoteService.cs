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
using DataModel.Client.Provider.Outgoing;
using DataModel.Client.Provider.Incoming;

namespace MiddleWare.Services
{
    public class NoteService : INoteService
    {
        private INoteRepository noteRepository;
        private IAppointmentRepository appointmentRepository;
        private IServiceRequestRepository serviceRequestRepository;
        private IMediaContainer mediaContainer;
        private ILogger logger;

        public NoteService(INoteRepository noteRepository, IAppointmentRepository appointmentRepository, IServiceRequestRepository serviceRequestRepository, IMediaContainer mediaContainer, ILogger<ReportService> logger)
        {
            this.noteRepository = noteRepository;
            this.appointmentRepository = appointmentRepository;
            this.serviceRequestRepository = serviceRequestRepository;
            this.mediaContainer = mediaContainer;
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
            }
        }

        public async Task<List<NoteOutgoing>> GetAllNotes(string organisationId, string customerId)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:GetAllNotes"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(organisationId, IdType.Organisation);
                DataValidation.ValidateObjectId(customerId, IdType.Customer);

                var notes = await noteRepository.GetAllNotes(organisationId, customerId);

                if (notes == null)
                {
                    logger.LogInformation("No reports available");
                    return new List<ProviderClientOutgoing.NoteOutgoing>();
                }

                return ServiceRequestConverter.ConvertToClientOutGoingNotes(notes);
            }
        }

        public async Task<List<NoteOutgoing>> GetAppointmentNote(string ServiceRequestId)
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

                return ServiceRequestConverter.ConvertToClientOutGoingNotes(notes);
            }

        }

        public async Task SetNote(NoteIncoming noteIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "NoteService:SetNote"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(noteIncoming.ServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(noteIncoming.AppointmentId, IdType.Appointment);

                if (!string.IsNullOrEmpty(noteIncoming.NoteId))
                {
                    DataValidation.ValidateObjectId(noteIncoming.NoteId, IdType.Note);
                }

                var note = ServiceRequestConverter.ConvertToMongoNote(noteIncoming);

                note.LastModifiedTime = DateTime.UtcNow;

                await noteRepository.AddNote(note, noteIncoming.ServiceRequestId);
            }
        }

        public async Task SetStrayNote(NoteIncoming noteIncoming, string AppointmentId, string ServiceRequestId)
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

                DataValidation.ValidateObjectId(AppointmentId, IdType.Appointment);
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                noteIncoming.ServiceRequestId = ServiceRequestId;
                noteIncoming.AppointmentId = AppointmentId;
                await SetNote(noteIncoming);
            }
        }
    }
}
