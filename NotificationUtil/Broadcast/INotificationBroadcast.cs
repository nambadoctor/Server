namespace NotificationUtil.Trigger
{
    public interface INotificationBroadcast
    {
        public Task FireAppointmentStatusNotification(string appointmentId);
        public Task FireReminderNotification(string appointmentId);
    }
}
