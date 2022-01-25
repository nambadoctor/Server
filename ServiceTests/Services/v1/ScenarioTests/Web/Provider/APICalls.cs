using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientCommon = DataModel.Client.Provider.Common;
using Newtonsoft.Json;

namespace ServiceTests.Services.v1.ScenarioTests.Web.Provider
{
    public class APICalls
    {
        private HttpClient httpClient;
        private string BaseUrl;

        public APICalls(HttpClient httpClient, string baseUrl)
        {
            this.httpClient = httpClient;
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            this.httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            BaseUrl = baseUrl;
        }

        #region GET
        public async Task<ProviderClientOutgoing.ServiceProviderBasic> GetServiceProviderOrganisationMemberships()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/serviceprovider"))
            {
                var response = await httpClient.SendAsync(request);
                var spBasic = JsonConvert.DeserializeObject<ProviderClientOutgoing.ServiceProviderBasic>(await response.Content.ReadAsStringAsync());
                return spBasic;
            }
        }

        public async Task<ProviderClientOutgoing.ServiceProvider> GetServiceProvider(string ServiceProviderId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/serviceprovider/{ServiceProviderId}/organisation/{OrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var sp = JsonConvert.DeserializeObject<ProviderClientOutgoing.ServiceProvider>(await response.Content.ReadAsStringAsync());
                return sp;
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetOrgAppointments(string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{OrganisationId}/appointments"))
            {

                var response = await httpClient.SendAsync(request);
                var appointments = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingAppointment>>(await response.Content.ReadAsStringAsync());
                return appointments;
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetOrgCustomers(string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{OrganisationId}/customers"))
            {
                var response = await httpClient.SendAsync(request);
                var customers = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingCustomerProfile>>(await response.Content.ReadAsStringAsync());
                return customers;
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingAppointment>> GetServiceProviderAppointments(string ServiceProviderId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{OrganisationId}/appointments?ServiceProviderIds={ServiceProviderId}"))
            {
                var response = await httpClient.SendAsync(request);
                var appointments = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingAppointment>>(await response.Content.ReadAsStringAsync());
                return appointments;
            }
        }

        public async Task<List<ProviderClientOutgoing.OutgoingCustomerProfile>> GetServiceProviderCustomers(string ServiceProviderId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/organisation/{OrganisationId}/customers?ServiceProviderIds={ServiceProviderId}"))
            {
                var response = await httpClient.SendAsync(request);
                var customers = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.OutgoingCustomerProfile>>(await response.Content.ReadAsStringAsync());
                return customers;
            }
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileById(string CustomerId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/customer/{CustomerId}/{OrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var customer = JsonConvert.DeserializeObject<ProviderClientOutgoing.OutgoingCustomerProfile>(await response.Content.ReadAsStringAsync());
                return customer;
            }
        }

        public async Task<ProviderClientOutgoing.OutgoingCustomerProfile> GetCustomerProfileByPhoneNumber(string PhoneNumber, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/customer/phonenumber/{PhoneNumber.Replace("+", "")}/{OrganisationId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<ProviderClientOutgoing.OutgoingCustomerProfile>(value);
                return customer;
            }
        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetAppointmentReports(string CustomerId, string ServiceRequestId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/report/{ServiceRequestId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var reports = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.ReportOutgoing>>(value);
                return reports;
            }
        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string CustomerId, string ServiceRequestId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/prescription/{ServiceRequestId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var prescriptions = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>>(value);
                return prescriptions;
            }
        }

        public async Task<List<ProviderClientOutgoing.ReportOutgoing>> GetCustomerReports(string CustomerId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/report/all/{OrganisationId}/{CustomerId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var reports = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.ReportOutgoing>>(value);
                return reports;
            }
        }

        public async Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetCustomerPrescriptions(string CustomerId, string OrganisationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"/prescription/all/{OrganisationId}/{CustomerId}"))
            {
                var response = await httpClient.SendAsync(request);
                var value = await response.Content.ReadAsStringAsync();
                var prescriptions = JsonConvert.DeserializeObject<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>>(value);
                return prescriptions;
            }
        }
        #endregion GET

        #region POST
        public async Task<bool> AddCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfileIncoming)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/customer"))
            {
                var jsonData = JsonConvert.SerializeObject(customerProfileIncoming);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> AddAppointment(ProviderClientIncoming.AppointmentIncoming appointmentIncoming)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/appointment"))
            {
                var jsonData = JsonConvert.SerializeObject(appointmentIncoming);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> AddCustomerWithAppointment(ProviderClientIncoming.CustomerProfileWithAppointmentIncoming customerProfileWithAppointmentIncoming)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/customer/appointment"))
            {
                var jsonData = JsonConvert.SerializeObject(customerProfileWithAppointmentIncoming);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> AddReport(ProviderClientIncoming.ReportIncoming report)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/report"))
            {
                var jsonData = JsonConvert.SerializeObject(report);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> AddPrescription(ProviderClientIncoming.PrescriptionDocumentIncoming prescription)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/prescription"))
            {
                var jsonData = JsonConvert.SerializeObject(prescription);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion POST

        #region PUT
        public async Task<bool> RescheduleAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl + "/appointment/reschedule"))
            {
                var jsonData = JsonConvert.SerializeObject(appointment);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> CancelAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl + "/appointment/cancel"))
            {
                var jsonData = JsonConvert.SerializeObject(appointment);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> EndAppointment(ProviderClientIncoming.AppointmentIncoming appointment)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl + "/appointment/end"))
            {
                var jsonData = JsonConvert.SerializeObject(appointment);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> UpdateCustomerProfile(ProviderClientIncoming.CustomerProfileIncoming customerProfileIncoming)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl + "/customer"))
            {
                var jsonData = JsonConvert.SerializeObject(customerProfileIncoming);
                var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");
                request.Content = contentData;
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var value = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion PUT

    }
}
