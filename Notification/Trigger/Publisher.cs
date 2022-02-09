using Notification.Trigger.EventArgs;

namespace Notification.Trigger;

public class Publisher
{
    public event EventHandler NewAppointment;
    public event EventHandler AppointmentRescheduled;
    public event EventHandler AppointmentCancelled;
    public event EventHandler NotifyCustomerDocumentAdded;
    public event EventHandler NotifyCustomerAdded;
    
    public void StartListenProcess()
    {
        Console.WriteLine("Appointment Process Started!");
        // some code here..
        OnNewAppointment();
        OnAppointmentRescheduled();
        OnAppointmentCancelled();
        OnDocumentAdded();
        OnCustomerAdded(new CustomerAddedEventArgs {CustomerName = "Test", PhoneNumber = "+917907144815"});
    }

    protected virtual void OnNewAppointment()
    {
        //if ProcessCompleted is not null then call delegate
        NewAppointment?.Invoke(this,System.EventArgs.Empty); 
    }
    
    protected virtual void OnAppointmentRescheduled()
    {
        //if ProcessCompleted is not null then call delegate
        AppointmentRescheduled?.Invoke(this,System.EventArgs.Empty); 
    }
    
    protected virtual void OnAppointmentCancelled()
    {
        //if ProcessCompleted is not null then call delegate
        AppointmentCancelled?.Invoke(this,System.EventArgs.Empty); 
    }
    
    protected virtual void OnDocumentAdded()
    {
        //if ProcessCompleted is not null then call delegate
        NotifyCustomerDocumentAdded?.Invoke(this,System.EventArgs.Empty); 
    }
    
    protected virtual void OnCustomerAdded(CustomerAddedEventArgs e)
    {
        //if ProcessCompleted is not null then call delegate
        NotifyCustomerAdded?.Invoke(this, e); 
    }
}