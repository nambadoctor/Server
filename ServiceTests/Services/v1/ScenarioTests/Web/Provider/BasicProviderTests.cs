using Microsoft.VisualStudio.TestTools.UnitTesting;
using NambaDoctorServiceTests.Services.Auth;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientCommon = DataModel.Client.Provider.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        TestInitializer testInitializer;

        [TestInitialize]
        public void Init()
        {
            testInitializer = TestInitializer.Instance;
            BaseUrl = TestInitializer.BaseUrl;
            apiCalls = TestInitializer.apiCalls;
            dataGeneration = TestInitializer.dataGeneration;
            httpClient = TestInitializer.httpClient;

            ChosenServiceProviderId = TestInitializer.ChosenServiceProviderId;
            ChosenOrganisationId = TestInitializer.ChosenOrganisationId;
        }

        [TestMethod]
        public async Task SignInTest()
        {
            var sp = await apiCalls.GetServiceProvider(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(sp);
        }

        [TestMethod]
        public async Task AppointmentsReadTest()
        {
            var OrgAppointmentsWithDateFilter = await apiCalls.GetOrgAppointments(ChosenOrganisationId, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1));
            Assert.IsNotNull(OrgAppointmentsWithDateFilter);

            var OrgAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            Assert.IsNotNull(OrgAppointments);

            var SpAppointments = await apiCalls.GetServiceProviderAppointments(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(SpAppointments);

            var bsAppointments = await apiCalls.GetOrgAppointments(ChosenServiceProviderId); //Wrong ID, expect empty list
            Assert.AreEqual(0, bsAppointments.Count);

            var bsSpAppointments = await apiCalls.GetServiceProviderAppointments(ChosenOrganisationId, ChosenOrganisationId); //Wrong ID, expect empty list
            Assert.AreEqual(0, bsSpAppointments.Count);
        }

        [TestMethod]
        public async Task CustomersReadTest()
        {

            var SpCustomers = await apiCalls.GetServiceProviderCustomers(ChosenOrganisationId);
            Assert.IsNotNull(SpCustomers);

            var chosenCustomer = ChooseRandomFromList(SpCustomers);
            var validPhoneNumber = chosenCustomer.PhoneNumbers.First();

            var customerFromId = await apiCalls.GetCustomerProfileById(chosenCustomer.CustomerId, ChosenOrganisationId);
            Assert.IsNotNull(customerFromId);

            var customerByPhone = await apiCalls.GetCustomerProfileByPhoneNumber(validPhoneNumber.CountryCode + validPhoneNumber.Number, ChosenOrganisationId);
            Assert.IsNotNull(customerByPhone);
        }

        [TestMethod]
        public async Task AppointmentPutTests()
        {
            var initialAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            var chosenAppointment = ChooseRandomFromList(initialAppointments);

            var existingAppointment = dataGeneration.GenerateSampleAppointment(ChosenServiceProviderId, ChosenOrganisationId, chosenAppointment.CustomerId, chosenAppointment.AppointmentId);

            var rescheduleResult = await apiCalls.RescheduleAppointment(existingAppointment);
            Assert.IsTrue(rescheduleResult);

            var endResult = await apiCalls.EndAppointment(existingAppointment);
            Assert.IsTrue(endResult);

            var cancelResult = await apiCalls.CancelAppointment(existingAppointment);
            Assert.IsTrue(cancelResult); //This will cause appointment to vanish on GetList

        }

        [TestMethod]
        public async Task AppointmentPostTests()
        {
            var customers = await apiCalls.GetServiceProviderCustomers(ChosenOrganisationId);
            var chosenCustomer = ChooseRandomFromList(customers);

            var newAppointment = dataGeneration.GenerateSampleAppointment(ChosenServiceProviderId, ChosenOrganisationId, chosenCustomer.CustomerId, "");
            var postAppointmentResult = await apiCalls.AddAppointment(newAppointment);
            Assert.IsTrue(postAppointmentResult);

            var currentAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            Assert.IsTrue(currentAppointments.Exists(appointment => appointment.CustomerId == newAppointment.CustomerId));

        }

        [TestMethod]
        public async Task CustomerPutTests()
        {
            var customers = await apiCalls.GetServiceProviderCustomers(ChosenOrganisationId);
            Assert.IsNotNull(customers);
            var rnd = new Random();
            var existingCustomer = customers.ElementAt(rnd.Next(customers.Count));
            var modifiedCustomer = dataGeneration.GenerateSampleCustomer(ChosenOrganisationId, existingCustomer.CustomerId, existingCustomer.CustomerProfileId, existingCustomer.PhoneNumbers);

            var updatCustomereResult = await apiCalls.UpdateCustomerProfile(modifiedCustomer);
            Assert.IsTrue(updatCustomereResult);

            var appointmentWithExistingCustomer = dataGeneration.GenerateSampleCustomerWithAppointment(ChosenServiceProviderId, ChosenOrganisationId, existingCustomer.CustomerId, existingCustomer.CustomerProfileId, existingCustomer.PhoneNumbers);
            var postAppointmentWithExistingCustomertResult = await apiCalls.AddCustomerWithAppointment(appointmentWithExistingCustomer); // Should throw error response
            Assert.IsFalse(postAppointmentWithExistingCustomertResult);

        }

        [TestMethod]
        public async Task CustomerPostTests()
        {
            var newCustomer = dataGeneration.GenerateSampleCustomer(ChosenOrganisationId, "", "", null);
            var postCustomerResult = await apiCalls.AddCustomerProfile(newCustomer);
            Assert.IsTrue(postCustomerResult);

            var appointmentWithNewCustomer = dataGeneration.GenerateSampleCustomerWithAppointment(ChosenServiceProviderId, ChosenOrganisationId, "", "", null);
            var postAppointmentWithNewCustomerResult = await apiCalls.AddCustomerWithAppointment(appointmentWithNewCustomer);
            Assert.IsTrue(postAppointmentWithNewCustomerResult);
        }

        [TestMethod]
        public async Task ReportRWTests()
        {
            var initialAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            var chosenAppointment = ChooseRandomFromList(initialAppointments);

            var report = dataGeneration.GenerateSampleReport(chosenAppointment.ServiceRequestId, chosenAppointment.AppointmentId);
            var postReportResult = await apiCalls.AddReport(report);
            Assert.IsTrue(postReportResult);

            var strayReport = dataGeneration.GenerateSampleReport("", "");
            var postStrayReportResult = await apiCalls.AddStrayReport(strayReport, ChosenOrganisationId, ChosenServiceProviderId, chosenAppointment.CustomerId);
            Assert.IsTrue(postStrayReportResult);

            var updatedReportList = await apiCalls.GetAppointmentReports(chosenAppointment.CustomerId, chosenAppointment.ServiceRequestId);
            Assert.IsTrue(updatedReportList.Count > 0);

            var reportToDelete = ChooseRandomFromList(updatedReportList);
            var reportDeleteResult = await apiCalls.DeleteReport(reportToDelete.ReportId);
            Assert.IsTrue(reportDeleteResult);

            var customerReports = await apiCalls.GetCustomerReports(chosenAppointment.CustomerId, ChosenOrganisationId);
            Assert.IsNotNull(customerReports);
        }

        [TestMethod]
        public async Task PrescriptionRWTests()
        {
            var initialAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            var chosenAppointment = ChooseRandomFromList(initialAppointments);

            var prescription = dataGeneration.GenerateSamplePrescription(chosenAppointment.ServiceRequestId, chosenAppointment.AppointmentId);
            var postPrescriptionResult = await apiCalls.AddPrescription(prescription);
            Assert.IsTrue(postPrescriptionResult);

            var strayPrescription = dataGeneration.GenerateSamplePrescription("", "");
            var postStrayPrescriptionResult = await apiCalls.AddStrayPrescription(strayPrescription, ChosenOrganisationId, ChosenServiceProviderId, chosenAppointment.CustomerId);
            Assert.IsTrue(postStrayPrescriptionResult);

            var updatedPrescriptionList = await apiCalls.GetAppointmentPrescriptions(chosenAppointment.CustomerId, chosenAppointment.ServiceRequestId);
            Assert.IsTrue(updatedPrescriptionList.Count > 0);

            var customerPrescriptions = await apiCalls.GetCustomerPrescriptions(chosenAppointment.CustomerId, ChosenOrganisationId);
            Assert.IsNotNull(customerPrescriptions);
        }

        [TestMethod]
        public async Task NotesTests()
        {
            var initialAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            var chosenAppointment = ChooseRandomFromList(initialAppointments);

            var note = dataGeneration.GenerateSampleNote(null, chosenAppointment.ServiceRequestId, chosenAppointment.AppointmentId);
            var postNoteResult = await apiCalls.AddNote(note);
            Assert.IsTrue(postNoteResult);

            var strayNote = dataGeneration.GenerateSampleNote(null, "", "");
            var postStrayNoteResult = await apiCalls.AddStrayNote(strayNote, ChosenOrganisationId, ChosenServiceProviderId, chosenAppointment.CustomerId);
            Assert.IsTrue(postStrayNoteResult);

            var updatedNoteList = await apiCalls.GetAppointmentNotes(chosenAppointment.ServiceRequestId);
            Assert.IsTrue(updatedNoteList.Count > 0);

            var customerNotes = await apiCalls.GetCustomerNotes(chosenAppointment.CustomerId, ChosenOrganisationId);
            Assert.IsNotNull(customerNotes);
        }

        [TestMethod]
        public async Task TreatmentPlanTests()
        {
            var initialAppointments = await apiCalls.GetOrgAppointments(ChosenOrganisationId);
            var chosenAppointment = ChooseRandomFromList(initialAppointments);

            var treatmentPlan = dataGeneration.GenerateTreatmentPlan(chosenAppointment.CustomerId, ChosenServiceProviderId, ChosenOrganisationId, chosenAppointment.ServiceRequestId, null);
            var postTreatmentPlanResult = await apiCalls.AddTreatmentPlan(treatmentPlan);
            Assert.IsTrue(postTreatmentPlanResult);

            var allTreatments = await apiCalls.GetAllTreatments(ChosenServiceProviderId, ChosenOrganisationId);
            Assert.IsNotNull(allTreatments);
            
            var treatmentPlans = await apiCalls.GetAllTreatmentPlans(ChosenOrganisationId, ChosenServiceProviderId);
            var chosenTreatmentPlan = ChooseRandomFromList(treatmentPlans);
            var chosenTreatment = ChooseRandomFromList(chosenTreatmentPlan.Treatments);

            var newTreatment = dataGeneration.GenerateTreatment(chosenAppointment.AppointmentId, chosenAppointment.ServiceRequestId, null);
            var postTreatmentResult = await apiCalls.AddTreatment(newTreatment, chosenTreatmentPlan.TreatmentPlanId);
            Assert.IsTrue(postTreatmentResult);

            var existingTreatment = dataGeneration.GenerateTreatment(chosenAppointment.AppointmentId, chosenAppointment.ServiceRequestId, chosenTreatment.TreatmentId);
            var putTreatmentResult = await apiCalls.UpdateTreatment(existingTreatment, chosenTreatmentPlan.TreatmentPlanId);
            Assert.IsTrue(putTreatmentResult);

            var updatedTreatmentPlan = dataGeneration.GenerateTreatmentPlan(chosenAppointment.CustomerId, ChosenServiceProviderId, ChosenOrganisationId, chosenAppointment.ServiceRequestId, chosenTreatmentPlan.TreatmentPlanId);
            var putTreatmentPlanResult = await apiCalls.UpdateTreatmentPlan(updatedTreatmentPlan);
            Assert.IsTrue(putTreatmentPlanResult);

        }

        private T ChooseRandomFromList<T>(List<T> list)
        {
            var rnd = new Random();

            return list.ElementAt(rnd.Next(0, list.Count));
        }
    }
}
