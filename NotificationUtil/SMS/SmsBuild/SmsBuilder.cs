using DataModel.Mongo.Notification;
using MongoDB.Bson;

namespace NotificationUtil.Mode.SMS;

public class SmsBuilder : ISmsBuilder
{
    public NotificationQueue GetAppointmentReminderSMS(string phoneNumber, DateTime time, string user, DateTime toBeNotifiedTime, string appointmentId)
    {
        var timeString = time.ToString("MMM dd, HH:mm").Trim().Replace(" ", "");
        String message = Uri.EscapeDataString($"Upcoming appointment. \nYour appointment with {user.Trim().Replace(" ", "")} is in {timeString}. Please be ready for the call.\n-Namba Doctor");
        return GetNotificationQueue(message, phoneNumber, toBeNotifiedTime, "NMBADR", NotificationType.Reminder, appointmentId);
    }

    public NotificationQueue GetAppointmentStatusSMS(string phoneNumber, DateTime time, string user, string status, DateTime toBeNotifiedTime, string appointmentId)
    {
        var timeString = time.ToString("MMM dd, HH:mm").Trim().Replace(" ", "");
        String message = Uri.EscapeDataString($"Appointment {status}.\nYour appointment on {timeString}(IST) with {user.Substring(0, Math.Min(user.Length, 10))} is {status}.\n-Namba Doctor");
        return GetNotificationQueue(message, phoneNumber, toBeNotifiedTime, "NmbaDr", NotificationType.AppointmentStatus, appointmentId);
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