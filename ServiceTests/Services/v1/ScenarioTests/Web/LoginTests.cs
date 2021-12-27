using Microsoft.VisualStudio.TestTools.UnitTesting;
using NambaDoctorServiceTests.Services.Auth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NambaDoctorServiceTests.Services.v1.ScenarioTests.Web
{
    [TestClass]
    public  class LoginTests
    {
        private string url = "https://localhost:44307/api/ServiceProvider";

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
                var response = await client.SendAsync(request);
            }
            string str = string.Empty;

        }
    }
}
