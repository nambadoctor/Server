using Notification.Medium.SMS;

namespace Notification;

public class Runner
{
    public static void Main(string[] args)
    {
        SendRandomSms();
    }

    public static SmsService GetSmsService()
    {
        return new SmsService(new SmsRepository(true));
    }

    public static void SendRandomSms()
    {
        GetSmsService().SendNewCustomerRegistrationSMS("+917901744815");
    }
}