namespace Notification.Trigger.EventArgs;

public class CustomerAddedEventArgs: System.EventArgs
{
    public string PhoneNumber { get; set; }
    public string CustomerName { get; set; }
}