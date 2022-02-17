namespace Notification.Trigger
{
    public interface IAppointmentStatusTrigger
    {
        public void FireAppointmentStatusNotification(string appointmentId);
    }
}
