using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NambaDoctorServiceTests.Services.Auth
{
    public class FirebaseAuthProvider
    {
        private string baseAddress = "https://identitytoolkit.googleapis.com";
        private string apiKey = "AIzaSyDHqNnrSfkxIos9FJQAsgWBVd1iBRvOa0g";

        public async Task<string> GetFBToken(string phonenumber, string otp)
        {
            var session = await GetFBSessionId(phonenumber);
            var authToken = await GetFBAuthInfo(session, otp);
            return authToken;
        }

        private async Task<string> GetFBAuthInfo(string sessionInfo, string otp)
        {
            var token = "";
            var path = "/v1/accounts:signInWithPhoneNumber";
            var payload = new
            {
                sessionInfo = sessionInfo,
                code = otp
            };
            var request = GenerateRequest(path, payload);

            try
            {
                var response = await GetResponse(request);
                token = JsonConvert.DeserializeObject<AuthInfo>(response).idToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} {ex.StackTrace}");
            }
            return token;
        }

        private async Task<string> GetFBSessionId(string phonenumber)
        {
            var session = "";
            var path = "/v1/accounts:sendVerificationCode";
            var payload = new
            {
                phoneNumber = phonenumber,
            };
            var request = GenerateRequest(path, payload);

            try
            {
                var response = await GetResponse(request);
                session = JsonConvert.DeserializeObject<SessionInfo>(response).sessionInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} {ex.StackTrace}");
            }
            return session;
        }

        private HttpWebRequest GenerateRequest(string path, object payload)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress + $"{path}?key={apiKey}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            return httpWebRequest;
        }
        private async Task<string> GetResponse(HttpWebRequest request)
        {
            var response = "-1";
            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }
            return response;
        }
    }
}
