using Microsoft.AspNetCore.Mvc;
using System;

namespace NambaDoctorWebApi.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            TimeSpan ts = TimeSpan.FromTicks(System.Environment.TickCount64);

            return $"UTC Time: {DateTime.UtcNow}. " +
                $"Elapsed Time: {ts.Days} days, {ts.Hours} hours, {ts.Minutes} minutes, {ts.Seconds} seconds. " +
                $"Machine:  {System.Environment.MachineName}. OS Version: {System.Environment.OSVersion}. CLR Version: {System.Environment.Version}. " +
                $"Working Set: {System.Environment.WorkingSet / 1024 / 1024} MB.";
        }
    }
}