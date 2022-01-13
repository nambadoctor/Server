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

        [HttpPut("")]
        [Authorize]
        public async Task SetAppointment([FromBody] ProviderClientIncoming.AppointmentIncoming appointment)
        {
            await appointmentService.SetAppointment(appointment);

        }
    }
}
