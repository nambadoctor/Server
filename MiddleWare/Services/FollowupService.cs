using DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using NotificationUtil.EventListener;

namespace MiddleWare.Services;

public class FollowupService: IFollowupService
{
    private INotificationEventListener notificationEventListener;
    private ILogger logger;

    public FollowupService(INotificationEventListener notificationEventListener, ILogger<FollowupService> logger)
    {
        this.notificationEventListener = notificationEventListener;
        this.logger = logger;
    }

    public async Task SetFollowup(FollowupIncoming followupIncoming)
    {
        using (logger.BeginScope("Method: {Method}", "FollowupService:SetFollowup"))
        using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
        {
            DataValidation.ValidateObjectId(followupIncoming.CustomerId, IdType.Customer);
            DataValidation.ValidateObjectId(followupIncoming.SenderServiceProviderId, IdType.ServiceProvider);

            await notificationEventListener.TriggerManualNotificationEvent(
                followupIncoming.CustomerId,
                followupIncoming.SenderServiceProviderId,
                followupIncoming.OrganisationId,
                "",
                followupIncoming.Reason,
                DataModel.Mongo.Notification.EventType.Followup,
                followupIncoming.ScheduledDateTime);

            logger.LogInformation($"Followup event triggered by {followupIncoming.SenderServiceProviderId}");
        }
    }
}