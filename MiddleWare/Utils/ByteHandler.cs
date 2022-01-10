namespace MiddleWare.Utils
{
    public class ByteHandler
    {
        public static byte[] Base64Decode(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
        }

        public static string Base64Encode(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }
    }
}
