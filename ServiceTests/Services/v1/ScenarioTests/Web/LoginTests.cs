using Microsoft.VisualStudio.TestTools.UnitTesting;
using NambaDoctorServiceTests.Services.Auth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NambaDoctorServiceTests.Services.v1.ScenarioTests.Web
{
    [TestClass]
    public  class LoginTests
    {
        private string url = "https://localhost:44307/api/provider/serviceprovider";

        private string GetAuthToken()
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider();
            var token = provider.GetFBToken("+919999999999", "123456").Result;
            return token;
        }

        [TestMethod]
        public async Task GetOrganisationList()
        {
            HttpClient client = new HttpClient();
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetAuthToken());
                request.Headers.Add("traceparent", Guid.NewGuid().ToString());

                //This will be set as OperationparentId for request log
                // For all traces need to get OperationId from above and get traces for all OperationId
                request.Headers.Add("phn", "+919999999999");

                // We can use the above for tracking a call E2E from client


                request.Headers.Add("spname", "+Manivannan Sengodan");
                request.Headers.Add("cv", "1.0");


                var response = await client.SendAsync(request);
            }
            string str = string.Empty;

        }
    }
}
