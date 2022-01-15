namespace MiddleWare.Utils
{
    public class ByteHandler
    {
        public static byte[] Base64Decode(string base64EncodedData)
        {
            string[] splitFileString = base64EncodedData.Split(',');
            byte[] decodedPrescription = Convert.FromBase64String(splitFileString.Last());
            return decodedPrescription;
        }

        public static string Base64Encode(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }
    }
}
