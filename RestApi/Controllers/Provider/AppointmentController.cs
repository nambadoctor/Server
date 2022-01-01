using DataModel.Client.Provider;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System;
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

        [HttpGet("{appointmentId}/{serviceproviderid}")]
        [Authorize]
        public async Task<Appointment> GetOrganisation(string AppointmentId, string ServiceProviderId)
        {

            if (string.IsNullOrWhiteSpace(AppointmentId))
            {
                throw new ArgumentException("Appointment Id was null");
            }

            var appointment = await appointmentService.GetAppointment(ServiceProviderId, AppointmentId);

            return appointment;
        }
    }
}
