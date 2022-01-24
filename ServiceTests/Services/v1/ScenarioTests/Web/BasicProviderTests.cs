using Microsoft.VisualStudio.TestTools.UnitTesting;
using NambaDoctorServiceTests.Services.Auth;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ServiceTests.Services.v1.ScenarioTests.Web.Provider
{
    [TestClass]
    public class BasicProviderTests
    {
        private string AuthToken;
        private HttpClient httpClient;
        private string BaseUrl = "https://localhost:44307/api/provider";

        private APICalls apiCalls;
        private DataGeneration dataGeneration;

        private string ChosenServiceProviderId = "";
        private string ChosenOrganisationId = "";
        private string ChosenCustomerId = "";
        private string ChosenAppointmentId = "";
        private string ChosenServiceRequestId = "";

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

            apiCalls = new APICalls(httpClient, BaseUrl);
            dataGeneration = new DataGeneration();
        }

        [TestMethod]
        public async Task RunProviderTests()
        {
            await RunProviderReadTests();

            await RunProviderWriteTests();
        }

        private async Task RunProviderReadTests()
        {
            SetAuthToken();
            InitHttpClient();

            var providerBasic = await apiCalls.GetServiceProviderOrganisationMemberships();
            Assert.IsNotNull(providerBasic);

            ChosenOrganisationId = providerBasic.Organisations.First().OrganisationId;
            ChosenServiceProviderId = providerBasic.ServiceProviderId;

            var sp = await apiCalls.GetServiceProvider(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(sp);

            var OrgAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            Assert.IsNotNull(OrgAppointments);

            var OrgCustomers = await apiCalls.GetOrgCustomers(ChosenOrganisationId);
            Assert.IsNotNull(OrgCustomers);

            var SpAppointments = await apiCalls.GetServiceProviderAppointments(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(SpAppointments);

            var SpCustomers = await apiCalls.GetServiceProviderCustomers(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(SpCustomers);

            var bsAppointments = await apiCalls.GetOrgAppointments(ChosenServiceProviderId); //Wrong ID, expect empty list
            Assert.AreEqual(0, bsAppointments.Count);

            var bsSpAppointments = await apiCalls.GetServiceProviderAppointments(ChosenOrganisationId, ChosenOrganisationId); //Wrong ID, expect empty list
            Assert.AreEqual(0, bsSpAppointments.Count);

            var chosenCustomer = OrgCustomers.FirstOrDefault();
            Assert.IsNotNull(chosenCustomer);
            ChosenCustomerId = chosenCustomer.CustomerId;
            var validPhoneNumber = chosenCustomer.PhoneNumbers.First();
            Assert.IsNotNull(validPhoneNumber);

            var chosenAppointment = OrgAppointments.FirstOrDefault();
            Assert.IsNotNull(chosenAppointment);
            ChosenAppointmentId = chosenAppointment.AppointmentId;
            ChosenServiceRequestId = chosenAppointment.ServiceRequestId;

            var customerFromId = await apiCalls.GetCustomerProfileById(ChosenCustomerId, ChosenOrganisationId);
            Assert.IsNotNull(customerFromId);
            var customerByPhone = await apiCalls.GetCustomerProfileByPhoneNumber(validPhoneNumber.CountryCode + validPhoneNumber.Number, ChosenOrganisationId);
            Assert.IsNotNull(customerByPhone);

            var reports = await apiCalls.GetAppointmentReports(ChosenCustomerId, ChosenServiceRequestId);
            var prescriptions = await apiCalls.GetAppointmentPrescriptions(ChosenCustomerId, ChosenServiceRequestId);

        }



        private async Task RunProviderWriteTests()
        {
            //PUT CALLS
            var existingAppointment = dataGeneration.GenerateSampleAppointment(ChosenServiceProviderId, ChosenOrganisationId, ChosenCustomerId, ChosenAppointmentId);

            var rescheduleResult = await apiCalls.RescheduleAppointment(existingAppointment);
            Assert.IsTrue(rescheduleResult);

            var endResult = await apiCalls.EndAppointment(existingAppointment);
            Assert.IsTrue(endResult);

            var cancelResult = await apiCalls.CancelAppointment(existingAppointment);
            Assert.IsTrue(cancelResult);

            var customers = await apiCalls.GetServiceProviderCustomers(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(customers);
            var rnd = new Random();
            var existingCustomer = customers.ElementAt(rnd.Next(customers.Count));
            var modifyCustomer = dataGeneration.GenerateSampleCustomer(ChosenServiceProviderId, ChosenOrganisationId, existingCustomer.CustomerId, existingCustomer.CustomerProfileId, existingCustomer.PhoneNumbers);

            var updatCustomereResult = await apiCalls.UpdateCustomerProfile(modifyCustomer);
            Assert.IsTrue(updatCustomereResult);

            //POST CALLS
            var newAppointment = dataGeneration.GenerateSampleAppointment(ChosenServiceProviderId, ChosenOrganisationId, ChosenCustomerId, "");
            var postAppointmentResult = await apiCalls.AddAppointment(newAppointment);
            Assert.IsTrue(postAppointmentResult);

            var newCustomer = dataGeneration.GenerateSampleCustomer(ChosenServiceProviderId, ChosenOrganisationId, "", "", null);
            var postCustomerResult = await apiCalls.AddCustomerProfile(newCustomer);
            Assert.IsTrue(postCustomerResult);

            var appointmentWithNewCustomer = dataGeneration.GenerateSampleCustomerWithAppointment(ChosenServiceProviderId, ChosenOrganisationId, "", "", null);
            var postAppointmentWithNewCustomerResult = await apiCalls.AddCustomerWithAppointment(appointmentWithNewCustomer);
            Assert.IsTrue(postAppointmentWithNewCustomerResult);

            var appointmentWithExistingCustomer = dataGeneration.GenerateSampleCustomerWithAppointment(ChosenServiceProviderId, ChosenOrganisationId, existingCustomer.CustomerId, existingCustomer.CustomerProfileId, existingCustomer.PhoneNumbers);
            var postAppointmentWithExistingCustomertResult = await apiCalls.AddCustomerWithAppointment(appointmentWithExistingCustomer); // Should throw error response
            Assert.IsFalse(postAppointmentWithExistingCustomertResult);

            var report = dataGeneration.GenerateSampleReport(ChosenServiceRequestId, ChosenAppointmentId);
            var postReportResult = await apiCalls.AddReport(report);
            Assert.IsTrue(postReportResult);

            var updatedReportList = await apiCalls.GetAppointmentReports(ChosenCustomerId, ChosenServiceRequestId);
            Assert.IsTrue(updatedReportList.Count > 0);

            var prescription = dataGeneration.GenerateSamplePrescription(ChosenServiceRequestId, ChosenAppointmentId);
            var postPrescriptionResult = await apiCalls.AddPrescription(prescription);
            Assert.IsTrue(postPrescriptionResult);

            var updatedPrescriptionList = await apiCalls.GetAppointmentPrescriptions(ChosenCustomerId, ChosenServiceRequestId);
            Assert.IsTrue(updatedPrescriptionList.Count > 0);
        }
    }
}
