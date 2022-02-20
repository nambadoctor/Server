namespace NotificationUtil.Trigger
{
    public interface INotificationBroadcast
    {
        public void FireAppointmentStatusNotification(string appointmentId);
        public void FireReminderNotification(string appointmentId);
    }
}
