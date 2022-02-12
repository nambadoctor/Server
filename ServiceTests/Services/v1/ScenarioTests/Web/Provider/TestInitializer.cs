using NambaDoctorServiceTests.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.Services.v1.ScenarioTests.Web.Provider
{
    public sealed class TestInitializer
    {
        private static TestInitializer instance = null;
        private static readonly object padlock = new object();
        public Boolean flag = false;

        public static HttpClient httpClient;
        public static string BaseUrl = "https://localhost:5001/api/provider";

        public static string ChosenServiceProviderId = "";
        public static string ChosenOrganisationId = "";
        public static APICalls apiCalls;
        public static DataGeneration dataGeneration;

        public static TestInitializer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TestInitializer();
                    }
                    return instance;
                }
            }
        }

        public async Task Initialize()
        {
            if (!flag)
            {
                InitHttpClient();
                await PopulateUserAndOrganisation();
                flag = true;
            }
            else
            {

            }

        }

        private string GetAuthToken()
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider();
            return provider.GetFBToken("+919999999999", "220272").Result;
        }

        private void InitHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAuthToken());

            apiCalls = new APICalls(httpClient, BaseUrl);
            dataGeneration = new DataGeneration();
        }

        private async Task PopulateUserAndOrganisation()
        {
            var providerBasic = await apiCalls.GetServiceProviderOrganisationMemberships();

            if (providerBasic != null)
            {
                ChosenOrganisationId = providerBasic.Organisations.First().OrganisationId;
                ChosenServiceProviderId = providerBasic.ServiceProviderId;
            }
        }

        public void CleanUp()
        {

        }
    }
}
