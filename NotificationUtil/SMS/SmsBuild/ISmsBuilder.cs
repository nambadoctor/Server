using DataModel.Mongo.Notification;

namespace NotificationUtil.Mode.SMS;

public interface ISmsBuilder
{
    public NotificationQueue GetAppointmentReminderSMS(string phoneNumber, DateTime time, string user, DateTime toBeNotifiedTime, string appointmentId, string orgName);

    public NotificationQueue GetAppointmentStatusSMS(string phoneNumber, DateTime time, string user, string status, DateTime toBeNotifiedTime, string appointmentId, string orgName);

    public NotificationQueue GetFutureAppointmentStatusSMS(string phoneNumber, DateTime time, string user, string status, DateTime toBeNotifiedTime, string appointmentId, string orgName);

    public NotificationQueue GetReferralSms(string phoneNumber, string patientName, string patientPhoneNumber, string drName, string orgName, string reason, DateTime dateTime);
    public NotificationQueue GetFollowupSms(string receiverPhoneNumber, string userName, string userPhoneNumber, string orgName, string reason, DateTime dateTime);
    //Deprecated
    public NotificationQueue GetPrescriptionSMS(string phoneNumber, string user, DateTime toBeNotifiedTime, string appointmentId);

    public NotificationQueue GetNewCustomerRegistrationSMS(string phoneNumber, DateTime toBeNotifiedTime);
}