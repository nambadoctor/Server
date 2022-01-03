using DataModel.Client.Provider;
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
        public async Task<Appointment> GetAppointment(string AppointmentId, string ServiceProviderId)
        {

            var appointment = await appointmentService.GetAppointment(ServiceProviderId, AppointmentId);

            return appointment;
        }

        [HttpPut()]
        public async Task<Appointment> SetAppointment([FromBody] Appointment appointment)
        {
            var appointmentToReturn = await appointmentService.SetAppointment(appointment);

            return appointmentToReturn;
        }
    }
}
