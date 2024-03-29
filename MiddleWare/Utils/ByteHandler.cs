﻿using System.Linq;

namespace MiddleWare.Utils
{
    public class ByteHandler
    {
        public static byte[] Base64DecodeFileString(string base64EncodedData)
        {
            string[] splitFileString = base64EncodedData.Split(',');
            byte[] decodedPrescription = Convert.FromBase64String(splitFileString.Last());
            return decodedPrescription;
        }

        public static string Base64Encode(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }
        public static string GetMimeType(string fileType)
        {
            if (fileType.Contains("jpeg")) return "image/jpeg";
            if (fileType.Contains("png")) return "image/png";
            if (fileType.Contains("pdf")) return "application/pdf";
            return "application/octet-stream";
            
        }
    }
}
