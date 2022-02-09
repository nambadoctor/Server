using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

namespace Notification.Medium.SMS;

public class SmsRepository: ISmsRepository
{
    private string APIKey = "ZWNhYThlY2I0ZTk5MzVkOTMxZTkxNDQ1MzhhM2I2NTI=";
    private string BaseUrl = "https://api.textlocal.in/send/";
    private bool isTest;
    private NameValueCollection nameValueCollection;
    private WebClient wb;
    
    public SmsRepository(bool isTest)
    {
        this.isTest = isTest;
        InitWebClient();
        InitBaseRequest();
    }
    
    private void InitWebClient()
    {
        wb = new WebClient();
    }

    private void InitBaseRequest()
    {
        nameValueCollection = new NameValueCollection()
        {
            {"num_parts", "1"},
            {"apikey", $"{APIKey}"},
            {"test", $"{isTest}"},
        };
    }

    public bool SendSms(string message, string phoneNumber, string senderId)
        {
            nameValueCollection.Set("message", message);
            nameValueCollection.Set("numbers", phoneNumber);
            nameValueCollection.Set("sender", senderId);
            
            using (var wb = this.wb)
            {
                byte[] response = wb.UploadValues(BaseUrl, nameValueCollection);
                string result = System.Text.Encoding.UTF8.GetString(response);
                try
                {
                    TextLocalResponse parsedResponse = JsonConvert.DeserializeObject<TextLocalResponse>(result);
                    //TODO Log reason and maybe retry
                    return parsedResponse?.status == "success";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
}