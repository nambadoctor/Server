using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace RestApi.Controllers.Provider;

[Route("api/provider/referral")]
[ApiController]
public class ReferralController : ControllerBase
{
    private readonly IReferralService referralService;
    public ReferralController(IReferralService referralService)
    {
        this.referralService = referralService;
    }

    [HttpPost]
    [Authorize]
    public async Task SetNewReferral(ProviderClientIncoming.ReferralIncoming referralIncoming)
    {
        await referralService.SetReferral(referralIncoming);
    }
}
