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
    [Route("api/provider/servicerequest")]
    [ApiController]
    public class ServiceRequestController
    {
        private NambaDoctorContext nambaDoctorContext;
        private IPrescriptionService prescriptionService;
        private IReportService reportService;
        private IServiceRequestService serviceRequestService;

        public ServiceRequestController(NambaDoctorContext nambaDoctorContext, IPrescriptionService prescriptionService, IReportService reportService, IServiceRequestService serviceRequestService)
        {
            this.nambaDoctorContext = nambaDoctorContext;
            this.prescriptionService = prescriptionService;
            this.reportService = reportService;
            this.serviceRequestService = serviceRequestService;
        }

    }
}
