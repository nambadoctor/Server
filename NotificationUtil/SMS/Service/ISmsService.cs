namespace NotificationUtil.Mode.SMS;

public interface ISmsService
{
    public bool SendAppointmentReminderSMS(string phoneNumber, string time, string user);

    public bool SendAppointmentStatusSMS(string phoneNumber, string time, string user, string status);

    public bool SendPrescriptionSMS(string phoneNumber, string user);

    public bool SendNewCustomerRegistrationSMS(string phoneNumber);
}