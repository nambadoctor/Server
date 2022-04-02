using DataModel.Client.Provider.Incoming;
using DataModel.Shared;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using NotificationUtil.EventListener;

namespace MiddleWare.Services
{
    public class ReferralService : IReferralService
    {
        private INotificationEventListener notificationEventListener;
        private ILogger logger;

        public ReferralService(INotificationEventListener notificationEventListener, ILogger<ReferralService> logger)
        {
            this.notificationEventListener = notificationEventListener;
            this.logger = logger;
        }
        public async Task SetReferral(ReferralIncoming referralIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "ReferralService:SetReferral"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(referralIncoming.CustomerId, IdType.Customer);
                DataValidation.ValidateObjectId(referralIncoming.SenderServiceProviderId, IdType.ServiceProvider);

                var phone = DataValidation.ExtractPhoneNumber(referralIncoming.PhoneNumber);

                await notificationEventListener.TriggerManualNotificationEvent(referralIncoming.CustomerId, referralIncoming.SenderServiceProviderId, referralIncoming.OrganisationId, phone, referralIncoming.Reason, DataModel.Mongo.Notification.EventType.Referred, DateTime.UtcNow);

                logger.LogInformation($"Referral event triggered to {phone} by {referralIncoming.SenderServiceProviderId}");
            }
        }
    }
}
