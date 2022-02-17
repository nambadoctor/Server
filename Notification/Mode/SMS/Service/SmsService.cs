namespace Notification.Mode.SMS;

public class SmsService : ISmsService
{
    private ISmsRepository smsRepository;

    public SmsService(ISmsRepository smsRepository)
    {
        this.smsRepository = smsRepository;
    }

    public bool SendAppointmentReminderSMS(string phoneNumber, string time, string user)
    {
        String message = Uri.EscapeDataString($"Upcoming appointment. \nYour appointment with {user.Trim().Replace(" ", "")} is in {time.Trim().Replace(" ", "")}. Please be ready for the call.\n-Namba Doctor");
        var response = smsRepository.SendSms(message, phoneNumber, "NMBADR");
        return response;
    }

    public bool SendAppointmentStatusSMS(string phoneNumber, string time, string user, string status)
    {
        String message = Uri.EscapeDataString($"Appointment {status}.\nYour appointment on {time}(IST) with {user.Substring(0, Math.Min(user.Length, 10))} is {status}.\n-Namba Doctor");
        var response = smsRepository.SendSms(message, phoneNumber, "NmbaDr");
        return response;
    }

    public bool SendPrescriptionSMS(string phoneNumber, string user)
    {
        String message = Uri.EscapeDataString($"Prescription added\nCheck out your prescription sent by {user}.\n-Namba Doctor ");
        var response = smsRepository.SendSms(message, phoneNumber, "NmbaDr");
        return response;
    }

    public bool SendNewCustomerRegistrationSMS(string phoneNumber)
    {
        String message = Uri.EscapeDataString($"Successfully registered on Namba Doctor!\nTo view your appointments, download the app at https://nambadoctor.page.link/app\n-Namba Doctor");
        var response = smsRepository.SendSms(message, phoneNumber, "NmbaDr");
        return response;
    }
}