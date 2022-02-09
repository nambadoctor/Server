using Notification.Medium.SMS;
using Notification.Trigger.EventArgs;

namespace Notification.Trigger;

public class Subscriber
{
    public static void Main()
    {
        Publisher pub = new Publisher();
        pub.NotifyCustomerAdded += sub_NewCustomer; // register with an event
        pub.StartListenProcess();
    }
    
    // event handler
    public static void sub_NewCustomer(object? sender, System.EventArgs e)
    {
        var arg = e as CustomerAddedEventArgs;
        Console.WriteLine($"New customer triggered!:{arg.CustomerName}");
        SendRandomSms((e as CustomerAddedEventArgs).PhoneNumber);
    }
    
    public static SmsService GetSmsService()
    {
        return new SmsService(new SmsRepository(true));
    }

    public static void SendRandomSms(string phoneNumber)
    {
        GetSmsService().SendNewCustomerRegistrationSMS(phoneNumber);
    }
    
}