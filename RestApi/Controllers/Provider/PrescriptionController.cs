using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/prescription")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private IPrescriptionService prescriptionService;
        private IAppointmentService appointmentService;

        public PrescriptionController(IPrescriptionService prescriptionService, IAppointmentService appointmentService)
        {
            this.prescriptionService = prescriptionService;
            this.appointmentService = appointmentService;
        }

        [HttpGet("{ServiceRequestId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptionDocuments(string ServiceRequestId)
        {

            var prescriptionDocuments = await prescriptionService.GetAppointmentPrescriptions(ServiceRequestId);

            return prescriptionDocuments;
        }

        [HttpGet("all/{OrganisationId}/{CustomerId}")]
        [Authorize]
        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetPrescriptions(string OrganisationId, string CustomerId)
        {

            var prescriptionDocuments = await prescriptionService.GetAllPrescriptions(OrganisationId, CustomerId);

            return prescriptionDocuments;
        }

        [HttpDelete("{PrescriptionDocumentId}")]
        [Authorize]
        public async Task DeletePrescriptionDocument(string PrescriptionDocumentId)
        {
            await prescriptionService.DeletePrescriptionDocument(PrescriptionDocumentId);
        }

        [HttpPost("")]
        [Authorize]
        public async Task SetPrescriptionDocument([FromBody] ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming)
        {
            await prescriptionService.SetPrescriptionDocument(prescriptionDocumentIncoming);
        }

        [HttpPost("Stray/{OrganisationId}/{ServiceProviderId}/{CustomerId}")]
        [Authorize]
        public async Task SetStrayPrescription([FromBody] ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming, string OrganisationId, string ServiceProviderId, string CustomerId)
        {

            var appointment = await appointmentService.UpsertAppointmentForStrayDocuments(OrganisationId, ServiceProviderId, CustomerId);
            await prescriptionService.SetStrayPrescription(prescriptionDocumentIncoming, appointment.AppointmentId.ToString(), appointment.ServiceRequestId);
        }
    }
}
