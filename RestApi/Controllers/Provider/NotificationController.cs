using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using Microsoft.AspNetCore.Mvc;
using MiddleWare.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace RestApi.Controllers.Provider;

[Route("api/provider/notification")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly IReferralService referralService;
    private readonly IFollowupService followupService;
    public NotificationController(IReferralService referralService, IFollowupService followupService)
    {
        this.referralService = referralService;
        this.followupService = followupService;
    }

    [HttpPost("referral")]
    [Authorize]
    public async Task SetNewReferral(ProviderClientIncoming.ReferralIncoming referralIncoming)
    {
        await referralService.SetReferral(referralIncoming);
    }
    
    [HttpPost("followup")]
    [Authorize]
    public async Task SetNewFollowup(ProviderClientIncoming.FollowupIncoming followupIncoming)
    {
        await followupService.SetFollowup(followupIncoming);
    }
}
