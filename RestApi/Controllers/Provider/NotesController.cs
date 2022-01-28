using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using MongoDB.GenericRepository.Interfaces;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {

        private INoteService noteService;
        private IAppointmentService appointmentService;

        public NoteController(INoteService noteService, IAppointmentService appointmentService)
        {
            this.noteService = noteService;
            this.appointmentService = appointmentService;
        }


        [HttpGet("{ServiceRequestId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.NoteOutgoing>> GetAppointmentNotes(string ServiceRequestId)
        {

            var notes = await noteService.GetAppointmentNote(ServiceRequestId);

            return notes;
        }

        [HttpGet("all/{OrganisationId}/{CustomerId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.NoteOutgoing>> GetNotes(string OrganisationId, string CustomerId)
        {

            var notes = await noteService.GetAllNotes(OrganisationId, CustomerId);

            return notes;
        }

        [HttpDelete("{ServiceRequestId}/{NoteId}")]
        [Authorize]
        public async Task DeleteNote(string ServiceRequestId, string NoteId)
        {
            await noteService.DeleteNote(ServiceRequestId, NoteId);
        }

        [HttpPost("")]
        [Authorize]
        public async Task SetNote([FromBody] ProviderClientIncoming.NoteIncoming noteIncoming)
        {
            await noteService.SetNote(noteIncoming);
        }

        [HttpPost("Stray/{OrganisationId}/{ServiceProviderId}/{CustomerId}")]
        [Authorize]
        public async Task SetStrayNote([FromBody] ProviderClientIncoming.NoteIncoming noteIncoming, string OrganisationId, string ServiceProviderId, string CustomerId)
        {
            var appointment = await appointmentService.UpsertAppointmentForStrayDocuments(OrganisationId, ServiceProviderId, CustomerId, DataModel.Mongo.AppointmentType.CustomerManagement);
            await noteService.SetStrayNote(noteIncoming, appointment.AppointmentId.ToString(), appointment.ServiceRequestId);
        }
    }
}
