using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientCommon = DataModel.Client.Provider.Common;

namespace ServiceTests.Services.v1.ScenarioTests.Web.Provider
{
    public class DataGeneration
    {
        public ProviderClientIncoming.AppointmentIncoming GenerateSampleAppointment(string ServiceProviderId, string OrganisationId, string CustomerId, string AppointmentId)
        {
            var appointment = new ProviderClientIncoming.AppointmentIncoming();

            appointment.ServiceProviderId = ServiceProviderId;
            appointment.OrganisationId = OrganisationId;
            appointment.CustomerId = CustomerId;
            appointment.AppointmentId = AppointmentId;

            appointment.Status = "Confirmed";
            appointment.AppointmentType = "InPerson";
            appointment.ScheduledAppointmentStartTime = DateTime.Now;
            appointment.ScheduledAppointmentEndTime = DateTime.Now.AddMinutes(20);

            return appointment;
        }

        public ProviderClientIncoming.CustomerProfileIncoming GenerateSampleCustomer(string ServiceProviderId, string OrganisationId, string CustomerId, string CustomerProfileId, List<ProviderClientCommon.PhoneNumber> custPhoneNumbers)
        {
            var customer = new ProviderClientIncoming.CustomerProfileIncoming();
            Random rnd = new Random();

            customer.ServiceProviderId = ServiceProviderId;
            customer.OrganisationId = OrganisationId;
            customer.CustomerId = CustomerId;
            customer.CustomerProfileId = CustomerProfileId;

            var num = rnd.Next(100);
            customer.FirstName = $"First{num}";
            customer.LastName = $"Last{num}";
            customer.DateOfBirth = new DataModel.Client.Provider.Common.DateOfBirth();
            customer.DateOfBirth.Age = $"{num}";
            customer.DateOfBirth.CreatedDate = DateTime.Now;
            customer.Gender = "Male";
            if (custPhoneNumbers == null)
            {
                var phoneNumbers = new List<ProviderClientCommon.PhoneNumber>();
                phoneNumbers.Add(new ProviderClientCommon.PhoneNumber { Type = "Primary", CountryCode = "+ND", Number = $"{rnd.Next(100000000, 999999999)}" });
                customer.PhoneNumbers = phoneNumbers;
            }
            else
            {
                customer.PhoneNumbers = custPhoneNumbers;
            }

            return customer;
        }

        public ProviderClientIncoming.CustomerProfileWithAppointmentIncoming GenerateSampleCustomerWithAppointment(string ServiceProviderId, string OrganisationId, string CustomerId, string CustomerProfileId, List<ProviderClientCommon.PhoneNumber> custPhoneNumbers)
        {
            Random rnd = new Random();

            var customerWithAppointment = new ProviderClientIncoming.CustomerProfileWithAppointmentIncoming();

            var customer = GenerateSampleCustomer(ServiceProviderId, OrganisationId, "", "", custPhoneNumbers);

            var appointment = GenerateSampleAppointment(ServiceProviderId, OrganisationId, "", "");

            customerWithAppointment.CustomerProfileIncoming = customer;
            customerWithAppointment.AppointmentIncoming = appointment;

            return customerWithAppointment;
        }

        public ProviderClientIncoming.ReportIncoming GenerateSampleReport(string ServiceRequestId, string AppointmentId)
        {
            var rnd = new Random();
            var report = new ProviderClientIncoming.ReportIncoming();

            report.FileType = "bs";
            report.FileName = $"report{rnd.Next(100)}";
            report.ServiceRequestId = ServiceRequestId;
            report.AppointmentId = AppointmentId;
            report.File = $"bs,cmVwb3J0";

            return report;
        }

        public ProviderClientIncoming.PrescriptionDocumentIncoming GenerateSamplePrescription(string ServiceRequestId, string AppointmentId)
        {
            var rnd = new Random();
            var prescription = new ProviderClientIncoming.PrescriptionDocumentIncoming();

            prescription.FileType = "bs";
            prescription.FileName = $"presc{rnd.Next(100)}";
            prescription.ServiceRequestId = ServiceRequestId;
            prescription.AppointmentId = AppointmentId;
            prescription.File = $"bs,cHJlc2NyaXB0aW9u";

            return prescription;
        }
    }
}
