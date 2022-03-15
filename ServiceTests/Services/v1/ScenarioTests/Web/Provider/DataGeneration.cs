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

        public ProviderClientIncoming.CustomerProfileIncoming GenerateSampleCustomer(string OrganisationId, string CustomerId, string CustomerProfileId, List<ProviderClientCommon.PhoneNumber> custPhoneNumbers)
        {
            var customer = new ProviderClientIncoming.CustomerProfileIncoming();
            Random rnd = new Random();

            customer.OrganisationId = OrganisationId;
            customer.CustomerId = CustomerId;
            customer.CustomerProfileId = CustomerProfileId;

            var num = rnd.Next(100);
            customer.FirstName = $"First{num}";
            customer.LastName = $"Last{num}";
            customer.DateOfBirth = new DataModel.Client.Provider.Common.DateOfBirth();
            customer.DateOfBirth.Date = DateTime.UtcNow.AddYears(-num);
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

            var customer = GenerateSampleCustomer(OrganisationId, "", "", custPhoneNumbers);

            var appointment = GenerateSampleAppointment(ServiceProviderId, OrganisationId, "", "");

            customerWithAppointment.CustomerProfileIncoming = customer;
            customerWithAppointment.AppointmentIncoming = appointment;

            return customerWithAppointment;
        }

        public ProviderClientIncoming.ReportIncoming GenerateSampleReport(string? ServiceRequestId, string? AppointmentId)
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
        
        public ProviderClientIncoming.TreatmentPlanDocumentIncoming GenerateSampleTreatmentPlanDocument(string treatmentPlanId)
        {
            var rnd = new Random();
            var tpDocument = new ProviderClientIncoming.TreatmentPlanDocumentIncoming();

            tpDocument.FileType = "bs";
            tpDocument.FileName = $"report{rnd.Next(100)}";
            tpDocument.File = $"bs,cmVwb3J0";
            tpDocument.TreatmentPlanId = treatmentPlanId;

            return tpDocument;
        }

        public ProviderClientIncoming.PrescriptionDocumentIncoming GenerateSamplePrescription(string? ServiceRequestId, string? AppointmentId)
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

        public ProviderClientIncoming.NoteIncoming GenerateSampleNote(string? NoteId, string? ServiceRequestId, string? AppointmentId)
        {
            var rnd = new Random();
            var note = new ProviderClientIncoming.NoteIncoming();

            if (!string.IsNullOrEmpty(NoteId))
            {
                note.NoteId = NoteId;
            }

            note.Note = $"Note {rnd.Next(100)}";

            note.ServiceRequestId = ServiceRequestId;
            note.AppointmentId = AppointmentId;

            return note;
        }

        public ProviderClientIncoming.TreatmentPlanIncoming GenerateTreatmentPlan(string CustomerId, string ServiceProviderId, string OrganisationId, string OriginServiceRequestId, string? treatmentPlanId)
        {
            var rnd = new Random();

            var treatmentPlan = new ProviderClientIncoming.TreatmentPlanIncoming();

            treatmentPlan.CustomerId = CustomerId;
            treatmentPlan.ServiceProviderId = ServiceProviderId;
            treatmentPlan.TreatmentPlanStatus = "InProgress";
            treatmentPlan.OrganisationId = OrganisationId;
            treatmentPlan.TreatmentPlanName = $"Treatment plan{rnd.Next(100)}";
            treatmentPlan.SourceServiceRequestId = OriginServiceRequestId;

            if (!string.IsNullOrWhiteSpace(treatmentPlanId))
            {
                treatmentPlan.TreatmentPlanId = treatmentPlanId;
            }

            treatmentPlan.Treatments = new List<ProviderClientIncoming.TreatmentIncoming>();

            for (var i = 0; i < rnd.Next(1, 5); i++)
            {
                treatmentPlan.Treatments.Add(GenerateTreatment(null, null, null));
            }

            return treatmentPlan;
        }

        public ProviderClientIncoming.TreatmentIncoming GenerateTreatment(string? AppointmentId, string? ServiceRequestId, string? TreatmentId)
        {
            var rnd = new Random();

            var treatment = new ProviderClientIncoming.TreatmentIncoming();

            treatment.Status = "Pending";
            treatment.Name = $"Treatment {rnd.Next(10)}";
            treatment.PlannedDateTime = DateTime.Now.AddDays(rnd.Next(10));
            treatment.OrginalInstructions = $"Instruction {rnd.Next(10)}";

            if (!string.IsNullOrWhiteSpace(TreatmentId))
            {
                treatment.TreatmentId = TreatmentId;
            }

            if (!string.IsNullOrWhiteSpace(AppointmentId))
            {
                treatment.AppointmentId = AppointmentId;
            }

            if (!string.IsNullOrWhiteSpace(ServiceRequestId))
            {
                treatment.ServiceRequestId = ServiceRequestId;
            }

            return treatment;
        }
    }
}
