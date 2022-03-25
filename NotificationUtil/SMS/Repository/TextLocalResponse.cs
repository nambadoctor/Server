namespace NotificationUtil.Mode.SMS;

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
    public long num_parts { get; set; }
    public string sender { get; set; }
    public string content { get; set; }
}

public class ReceiverInfo
{
    public long id { get; set; }
    public long recipient { get; set; }
}

public class Warning
{
    public long code { get; set; }
    public string message { get; set; }
}

public class Error
{
    public long code { get; set; }
    public string message { get; set; }
}

public class TextLocalResponse
{
    public bool test_mode { get; set; }
    public long balance { get; set; }
    public long batch_id { get; set; }
    public long cost { get; set; }
    public long num_messages { get; set; }
    public Message message { get; set; }
    public string receipt_url { get; set; }
    public string custom { get; set; }
    public List<ReceiverInfo> messages { get; set; }
    public List<Warning> warnings { get; set; }
    public List<Error> errors { get; set; }
    public string status { get; set; }
}