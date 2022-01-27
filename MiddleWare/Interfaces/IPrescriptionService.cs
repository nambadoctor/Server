﻿using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;

namespace MiddleWare.Interfaces
{
    public interface IPrescriptionService
    {
        public Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAllPrescriptions(string organisationId, string customerId);
        public Task<List<ProviderClientOutgoing.PrescriptionDocumentOutgoing>> GetAppointmentPrescriptions(string ServiceRequestId);
        public Task SetPrescriptionDocument(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming);
        public Task SetStrayPrescription(ProviderClientIncoming.PrescriptionDocumentIncoming prescriptionDocumentIncoming, string OrganisationId, string ServiceProviderId, string CustomerId);
        public Task DeletePrescriptionDocument(string ServiceRequestId, string PrescriptionDocumentId);
    }
}
