using AdminClientOutgoing = DataModel.Client.Admin.Outgoing;
using DataModel.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RestApi.Controllers.Admin
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private NambaDoctorContext nambaDoctorContext;
        private IAdminService adminService;

        public AdminController(NambaDoctorContext nambaDoctorContext, IAdminService adminService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.adminService = adminService;
        }

        [HttpGet("stats")]
        [Authorize]
        public async Task<List<AdminClientOutgoing.OutgoingAdminStat>> GetAppointment()
        {
            if (NambaDoctorContext.PhoneNumber != "+911234567890")
            {
                throw new UnauthorizedAccessException($"Admin api tried to access with phone: {NambaDoctorContext.PhoneNumber}");
            }

            var stats = await adminService.GetAdminStats();

            return stats;
        }
    }
}
