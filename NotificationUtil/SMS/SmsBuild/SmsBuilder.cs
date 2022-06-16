using DataModel.Mongo.Notification;
using MongoDB.Bson;

namespace NotificationUtil.Mode.SMS;

public class SmsBuilder : ISmsBuilder
{
    private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
    public NotificationQueue GetAppointmentReminderSMS(string phoneNumber, DateTime time, string user, DateTime toBeNotifiedTime, string appointmentId, string orgName)
    {
        var timeString = TimeZoneInfo.ConvertTimeFromUtc(time, INDIAN_ZONE).ToString("MMM dd,HH:mm").Trim();
        String message = Uri.EscapeDataString($"Reminder.\nYour appointment with {user.Substring(0, Math.Min(user.Length, 19))} at {orgName.Substring(0, Math.Min(orgName.Length, 14))} is on {timeString}.\n-Powered by Namba Doctor");
        String formattedStr = message.Replace("%0A", "%n");
        return GetNotificationQueue(formattedStr, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.Reminder, appointmentId);
    }

    public NotificationQueue GetAppointmentStatusSMS(string phoneNumber, DateTime time, string user, string status, DateTime toBeNotifiedTime, string appointmentId, string orgName)
    {
        var timeString = TimeZoneInfo.ConvertTimeFromUtc(time, INDIAN_ZONE).ToString("MMM dd,HH:mm").Trim();
        var strMsg = $"Appointment {status}.\nYour appointment on {timeString} with {user.Substring(0, Math.Min(user.Length, 19))} at {orgName.Substring(0, Math.Min(orgName.Length, 14))} has been {status.ToLower()}. \n-Powered by Namba Doctor";
        String msg = Uri.EscapeDataString(strMsg);
        String formattedStr = msg.Replace("%0A", "%n");
        return GetNotificationQueue(formattedStr, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.AppointmentStatus, appointmentId);
    }

    public NotificationQueue GetFutureAppointmentStatusSMS(string phoneNumber, DateTime time, string user, string status, DateTime toBeNotifiedTime, string appointmentId, string orgName)
    {
        var timeString = TimeZoneInfo.ConvertTimeFromUtc(time, INDIAN_ZONE).ToString("MMM dd,HH:mm").Trim();
        String msg = Uri.EscapeDataString($"Appointment {status}.\nYou are due for your appointment with {user.Substring(0, Math.Min(user.Length, 19))} on {timeString} at {orgName.Substring(0, Math.Min(orgName.Length, 10))}. \n-Powered by Namba Doctor");
        String formattedStr = msg.Replace("%0A", "%n");
        return GetNotificationQueue(formattedStr, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.AppointmentStatus, appointmentId);
    }

    public NotificationQueue GetReferralSms(string phoneNumber, string patientName, string patientPhoneNumber, string drName, string orgName, string reason, DateTime dateTime)
    {
        //TODO Update with right template
        String msg = Uri.EscapeDataString($"New Referral.\n {patientName.Substring(0, Math.Min(patientName.Length, 17))}.\nPh: {patientPhoneNumber}.\nReason: {reason.Substring(0, Math.Min(reason.Length, 24))}\nReferred by: {drName.Substring(0, Math.Min(drName.Length, 14))} from {orgName.Substring(0, Math.Min(orgName.Length, 9))}.\n- Powered by Namba Doctor.");
        String formattedStr = msg.Replace("%0A", "%n");
        return GetNotificationQueue(formattedStr, phoneNumber, dateTime, "NmbaDr", NotificationType.Referral, "");
    }

    public NotificationQueue GetFollowupSms(string receiverPhoneNumber, string userName, string userPhoneNumber, string orgName,
        string reason, DateTime dateTime)
    {
        var timeString = TimeZoneInfo.ConvertTimeFromUtc(dateTime, INDIAN_ZONE).ToString("MMM dd,HH:mm").Trim();
        String msg = Uri.EscapeDataString($"Review scheduled.\nTime of appointment: {timeString}.\nWith: {userName}.\nLocation: {orgName}.\n- Powered by Namba Doctor");
        String formattedStr = msg.Replace("%0A", "%n");
        return GetNotificationQueue(formattedStr, receiverPhoneNumber, dateTime, "NmbaDr", NotificationType.Followup, "");
    }

    public NotificationQueue GetPrescriptionSMS(string phoneNumber, string user, DateTime toBeNotifiedTime, string appointmentId)
    {
        String message = Uri.EscapeDataString($"Prescription added\nCheck out your prescription sent by {user}.\n-Namba Doctor ");
        return GetNotificationQueue(message, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.PrescriptionUploaded, appointmentId);
    }

    public NotificationQueue GetNewCustomerRegistrationSMS(string phoneNumber, DateTime toBeNotifiedTime)
    {
        String message = Uri.EscapeDataString($"Successfully registered on Namba Doctor!\nTo view your appointments, download the app at https://nambadoctor.page.link/app\n-Namba Doctor");
        return GetNotificationQueue(message, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.NewCustomer, null);
    }

    private NotificationQueue GetNotificationQueue(string message, string phoneNumber, DateTime toBeNotifiedTime, string senderId, NotificationType notificationType, string? appointmentId)
    {
        var notificationQueue = new NotificationQueue();

        notificationQueue.NotificationQueueId = ObjectId.GenerateNewId();

        notificationQueue.CreatedDateTime = DateTime.UtcNow;

        notificationQueue.Message = message;

        notificationQueue.NotificationType = notificationType;

        notificationQueue.UserPhoneNumber = phoneNumber;

        notificationQueue.SenderId = senderId;

        notificationQueue.ToBeNotifiedTime = toBeNotifiedTime;

        notificationQueue.AppointmentId = appointmentId;

        return notificationQueue;
    }
}