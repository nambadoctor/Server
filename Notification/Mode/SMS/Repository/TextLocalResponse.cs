namespace Notification.Mode.SMS;

// public class TextLocalResponse
// {
//     public List<Error> errors{ get; }
//     public string status{ get; }//success/failure
// }
//
// public class Error
// {
//     public int code { get; }
//     public string message { get; }
// }

public class Message
{
    public int num_parts { get; set; }
    public string sender { get; set; }
    public string content { get; set; }
}

public class ReceiverInfo
{
    public int id { get; set; }
    public long recipient { get; set; }
}

public class Warning
{
    public int code { get; set; }
    public string message { get; set; }
}

public class Error
{
    public int code { get; set; }
    public string message { get; set; }
}

public class TextLocalResponse
{
    public bool test_mode { get; set; }
    public int balance { get; set; }
    public int batch_id { get; set; }
    public int cost { get; set; }
    public int num_messages { get; set; }
    public Message message { get; set; }
    public string receipt_url { get; set; }
    public string custom { get; set; }
    public List<ReceiverInfo> messages { get; set; }
    public List<Warning> warnings { get; set; }
    public List<Error> errors { get; set; }
    public string status { get; set; }
}