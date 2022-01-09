using Microsoft.VisualStudio.TestTools.UnitTesting;
using NambaDoctorServiceTests.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace ServiceTests.Services.v1.ScenarioTests.Web
{
    [TestClass]
    public class BasicProviderTests
    {
        private string AuthToken;
        private HttpClient httpClient;
        private string BaseUrl = "https://localhost:44307/api/provider";

        private string ChosenServiceProviderId = "";
        private string ChosenOrganisationId = "";
        private string ChosenCustomerId = "";
        private string ChosenAppointmentId = "";

        private void SetAuthToken()
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider();
            AuthToken = provider.GetFBToken("+911234567890", "123456").Result;
        }

        private void InitHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
        }

        private async Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemberships()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/serviceprovider"))
            {
                var response = await httpClient.SendAsync(request);
                var spBasic = JsonConvert.DeserializeObject<ProviderClientOutgoing.ServiceProviderBasic>(await response.Content.ReadAsStringAsync());
                return spBasic;
            }
        }

        private async Task<ProviderClientOutgoing.ServiceProvider> GetServiceProvider()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/serviceprovider/{ChosenServiceProviderId}/organisation/{ChosenOrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var sp = JsonConvert.DeserializeObject<ProviderClientOutgoing.ServiceProvider>(await response.Content.ReadAsStringAsync());
                return sp;
            }
        }

        private async Task<List<ProviderClientOutgoing.GeneratedSlot>> GetServiceProviderSlots()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/serviceprovider/{ChosenServiceProviderId}/slots/{ChosenOrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var slots = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.GeneratedSlot>>(await response.Content.ReadAsStringAsync());
                return slots;
            }
        }

        private async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetOrgAppointments()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{ChosenOrganisationId}/appointments"))
            {

                var response = await httpClient.SendAsync(request);
                var appointments = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingAppointment>>(await response.Content.ReadAsStringAsync());
                return appointments;
            }
        }

        private async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetOrgCustomers()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{ChosenOrganisationId}/customers"))
            {
                var response = await httpClient.SendAsync(request);
                var customers = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingCustomerProfile>>(await response.Content.ReadAsStringAsync());
                return customers;
            }
        }

        private async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetServiceProviderAppointments()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{ChosenOrganisationId}/appointments?ServiceProviderIds={ChosenServiceProviderId}"))
            {
                var response = await httpClient.SendAsync(request);
                var appointments = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingAppointment>>(await response.Content.ReadAsStringAsync());
                return appointments;
            }
        }

        private async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetServiceProviderCustomers()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{ChosenOrganisationId}/customers?ServiceProviderIds={ChosenServiceProviderId}"))
            {
                var response = await httpClient.SendAsync(request);
                var customers = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingCustomerProfile>>(await response.Content.ReadAsStringAsync());
                return customers;
            }
        }

        private async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileById()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/customer/{ChosenCustomerId}/{ChosenOrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var customer = JsonConvert.DeserializeObject<ProviderClientOutgoing.OutgoingCustomerProfile>(await response.Content.ReadAsStringAsync());
                return customer;
            }
        }

        private async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileByPhoneNumber(string PhoneNumber)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/customer/CheckByPhoneNumber/{PhoneNumber.Replace("+", "")}/{ChosenOrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<ProviderClientOutgoing.OutgoingCustomerProfile>(value);
                return customer;
            }
        }

        [TestMethod]
        public async Task RunProviderReadTests()
        {
            SetAuthToken();
            InitHttpClient();
            var providerBasic = await GetServiceProviderOrganisationMemberships();
            ChosenOrganisationId = providerBasic.Organsiations.First().OrganisationId;
            ChosenServiceProviderId = providerBasic.ServiceProviderId;
            var sp = await GetServiceProvider();
            var slots = await GetServiceProviderSlots();
            var OrgAppointments = await GetOrgAppointments();
            var OrgCustomers = await GetOrgCustomers();
            var SpAppointments = await GetServiceProviderAppointments();
            var SpCustomers = await GetServiceProviderCustomers();

            var chosenCustomer = OrgCustomers.FirstOrDefault();
            ChosenCustomerId = chosenCustomer.CustomerId;

            var validPhoneNumber = chosenCustomer.PhoneNumbers.First();

            var customerFromId = await GetCustomerProfileById();
            var customerByPhone = await GetCustomerProfileByPhoneNumber(validPhoneNumber.CountryCode + validPhoneNumber.Number);

        }
    }
}
