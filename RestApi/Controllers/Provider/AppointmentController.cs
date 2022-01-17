using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;

namespace RestApi.Controllers.Provider
{
    [Route("api/provider/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private IAppointmentService appointmentService;

        public AppointmentController(NambaDoctorContext nambaDoctorContext, IAppointmentService appointmentService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.appointmentService = appointmentService;
        }

        [HttpGet("{AppointmentId}/{Serviceproviderid}")]
        [Authorize]
        public async Task<ProviderClientOutgoing.OutgoingAppointment> GetAppointment(string AppointmentId, string ServiceProviderId)
        {

            var appointment = await appointmentService.GetAppointment(ServiceProviderId, AppointmentId);

            return appointment;
        }

        [HttpPost("")]
        [Authorize]
        public async Task AddAppointment([FromBody] ProviderClientIncoming.AppointmentIncoming appointment)
        {
            await appointmentService.AddAppointment(appointment);

        }

        [HttpPut("reschedule")]
        [Authorize]
        public async Task RescheduleAppointment([FromBody] ProviderClientIncoming.AppointmentIncoming appointment)
        {
            await appointmentService.RescheduleAppointment(appointment);

        }

        [HttpPut("cancel")]
        [Authorize]
        public async Task CancelAppointment([FromBody] ProviderClientIncoming.AppointmentIncoming appointment)
        {
            await appointmentService.CancelAppointment(appointment);

        }

        [HttpPut("end")]
        [Authorize]
        public async Task EndAppointment([FromBody] ProviderClientIncoming.AppointmentIncoming appointment)
        {
            await appointmentService.EndAppointment(appointment);

        }
    }
}
