namespace NotificationUtil.Mode.SMS;

public interface ISmsRepository
{
    public bool SendSms(string message, string phoneNumber, string senderId);
}