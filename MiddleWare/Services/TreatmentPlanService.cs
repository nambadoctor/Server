using DataModel.Client.Provider.Incoming;
using DataModel.Client.Provider.Outgoing;
using DataModel.Shared;
using MiddleWare.Converters;
using MiddleWare.Interfaces;
using MiddleWare.Utils;
using MongoDB.Bson;
using Exceptions = DataModel.Shared.Exceptions;
using MongoDB.GenericRepository.Interfaces;
using ND.DataLayer.Utils.BlobStorage;
using Mongo = DataModel.Mongo;
using ProviderClientIncoming = DataModel.Client.Provider.Incoming;
using ProviderClientOutgoing = DataModel.Client.Provider.Outgoing;

namespace MiddleWare.Services
{
    public class TreatmentPlanService : ITreatmentPlanService
    {
        private ILogger logger;
        private ITreatmentPlanRepository treatmentPlanRepository;
        private IAppointmentRepository appointmentRepository;
        private IServiceProviderRepository serviceProviderRepository;
        private ICustomerRepository customerRepository;
        private IMediaContainer mediaContainer;

        public TreatmentPlanService(ITreatmentPlanRepository treatmentPlanRepository, IAppointmentRepository appointmentRepository, ILogger<TreatmentPlanService> logger, IServiceProviderRepository serviceProviderRepository, ICustomerRepository customerRepository, IMediaContainer mediaContainer)
        {
            this.treatmentPlanRepository = treatmentPlanRepository;
            this.appointmentRepository = appointmentRepository;
            this.serviceProviderRepository = serviceProviderRepository;
            this.customerRepository = customerRepository;
            this.mediaContainer = mediaContainer;
            this.logger = logger;
        }

        public async Task AddTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:AddTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);

                var mongoTreatment = TreatmentPlanConverter.ConvertToMongoTreatment(treatmentIncoming);

                logger.LogInformation("Constructed mongo treatment plan obj");

                await treatmentPlanRepository.AddTreatment(TreatmentPlanId, mongoTreatment);

                logger.LogInformation($"Added treatment with id: {mongoTreatment.TreatmentId}");
            }
        }

        public async Task<List<TreatmentOutgoing>> GetTreatments(string OrganisationId, string ServiceproviderId, string CustomerId, bool IsUpcoming)
        {
            DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

            if (!string.IsNullOrWhiteSpace(CustomerId))
            {
                DataValidation.ValidateObjectId(CustomerId, IdType.Customer);
            }
            
            List<Mongo.TreatmentPlan> mongoTreatmentPlans = await treatmentPlanRepository.GetAllTreatmentPlans(OrganisationId, ServiceproviderId, CustomerId);
            
            logger.LogInformation($"Received {mongoTreatmentPlans.Count} treatment plans from db");

            var outgoingTreatments = TreatmentPlanConverter.ConvertToDenormalizedTreatments(mongoTreatmentPlans);
            
            if (IsUpcoming)
            {
                FilterTreatmentPlans(ref outgoingTreatments);
            }

            logger.LogInformation("Converted treatments to outgoing successfully");

            return outgoingTreatments;
        }

        public async Task AddTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:AddTreatmentPlan"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(treatmentPlanIncoming.ServiceProviderId, IdType.ServiceProvider);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.CustomerId, IdType.Customer);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.SourceServiceRequestId, IdType.ServiceRequest);
                DataValidation.ValidateObjectId(treatmentPlanIncoming.OrganisationId, IdType.Organisation);

                var customerProfile = await customerRepository.GetCustomerProfile(treatmentPlanIncoming.CustomerId, treatmentPlanIncoming.OrganisationId);
                DataValidation.ValidateObject(customerProfile);

                var serviceProviderProfile = await serviceProviderRepository.GetServiceProviderProfile(treatmentPlanIncoming.ServiceProviderId, treatmentPlanIncoming.OrganisationId);
                DataValidation.ValidateObject(serviceProviderProfile);

                var mongoTreatmentPlan = TreatmentPlanConverter.ConvertToMongoTreatmentPlan(
                    treatmentPlanIncoming,
                    $"{serviceProviderProfile.FirstName} {serviceProviderProfile.LastName}",
                    $"{customerProfile.FirstName} {customerProfile.LastName}"
                    );

                logger.LogInformation("Constructed mongo treatment plan obj");

                await treatmentPlanRepository.Add(mongoTreatmentPlan);

                logger.LogInformation($"Created treatment plan with id: {mongoTreatmentPlan.TreatmentPlanId}");
            }
        }

        public async Task DeleteTreatment(string TreatmentPlanId, string TreatmentId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:DeleteTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);
                DataValidation.ValidateObjectId(TreatmentId, IdType.TreatmentPlan);

                await treatmentPlanRepository.RemoveTreatment(TreatmentPlanId, TreatmentId);

                logger.LogInformation($"Deleted treatment with id: {TreatmentId}");
            }
        }

        public async Task<List<TreatmentPlanDocumentsOutgoing>> GetTreatmentPlanDocuments(string ServiceRequestId)
        {
            
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:GetTreatmentPlanDocuments"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(ServiceRequestId, IdType.ServiceRequest);

                var treatmentPlan = await treatmentPlanRepository.GetTreatmentPlanByServiceRequestId(ServiceRequestId);

                var treatmentPlanDocuments = new List<TreatmentPlanDocumentsOutgoing>();

                if (treatmentPlan == null)
                    return treatmentPlanDocuments;
                
                if (treatmentPlan.UploadedDocuments == null || treatmentPlan.UploadedDocuments.Count == 0)
                {
                    return treatmentPlanDocuments;
                }

                foreach (var document in treatmentPlan.UploadedDocuments)
                {
                    var sasUrl = await mediaContainer.GetSasUrl(document.FileInfoId.ToString());

                    if (sasUrl != null)
                    {
                        treatmentPlanDocuments.Add(
                            TreatmentPlanConverter.ConvertToClientOutgoingTreatmentPlanDocument(document, sasUrl, treatmentPlan.TreatmentPlanId.ToString(), treatmentPlan.SourceServiceRequestId)
                        );
                    }
                    else
                    {
                        throw new Exceptions.BlobStorageException($"Treatment plan not found in blob:{treatmentPlan.TreatmentPlanId.ToString()}");
                    }
                }

                return treatmentPlanDocuments;

            }
        }

        public async Task<List<TreatmentPlanDocumentsOutgoing>> GetTreatmentPlanDocumentsOfCustomer(string CustomerId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:GetTreatmentPlanDocuments"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(CustomerId, IdType.Customer);

                var treatmentPlans = await treatmentPlanRepository.GetTreatmentPlansByCustomerId(CustomerId);

                var treatmentPlanDocuments = new List<TreatmentPlanDocumentsOutgoing>();

                if (treatmentPlans == null || treatmentPlans.Count == 0)
                    return treatmentPlanDocuments;

                foreach (var treatmentPlan in treatmentPlans)
                {
                    if (treatmentPlan.UploadedDocuments == null || treatmentPlan.UploadedDocuments.Count == 0)
                    {
                        continue;
                    }
                    
                    foreach (var document in treatmentPlan.UploadedDocuments)
                    {
                        var sasUrl = await mediaContainer.GetSasUrl(document.FileInfoId.ToString());

                        if (sasUrl != null)
                        {
                            treatmentPlanDocuments.Add(
                                TreatmentPlanConverter.ConvertToClientOutgoingTreatmentPlanDocument(document, sasUrl, treatmentPlan.TreatmentPlanId.ToString(), treatmentPlan.SourceServiceRequestId)
                            );
                        }
                        else
                        {
                            throw new Exceptions.BlobStorageException($"Treatment plan not found in blob:{treatmentPlan.TreatmentPlanId.ToString()}");
                        }
                    }
                }

                return treatmentPlanDocuments;

            }
        }

        public async Task SetTreatmentPlanDocument(TreatmentPlanDocumentIncoming treatmentPlanDocumentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:SetTreatmentPlanDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                //Validations
                
                DataValidation.ValidateObjectId(treatmentPlanDocumentIncoming.AppointmentId,
                    IdType.Appointment);
                DataValidation.ValidateObjectId(treatmentPlanDocumentIncoming.ServiceRequestId,
                    IdType.ServiceRequest);

                var existingTreatmentPlan =
                    await treatmentPlanRepository.GetTreatmentPlanByServiceRequestId(treatmentPlanDocumentIncoming.ServiceRequestId);

                var treatmentPlanIdToWriteTo = "";

                if (existingTreatmentPlan == null)
                {
                    var appointment =
                        await appointmentRepository.GetAppointment(treatmentPlanDocumentIncoming.AppointmentId);
                    //Create new treatment plan with appointment details
                    treatmentPlanIdToWriteTo = await CreateNewBlankTreatmentPlan(appointment);

                    logger.LogInformation($"Created new treatment plan with id: {treatmentPlanIdToWriteTo}");
                }
                else
                {
                    treatmentPlanIdToWriteTo = existingTreatmentPlan.TreatmentPlanId.ToString();
                }

                var treatmentPlanDocument = TreatmentPlanConverter.ConvertToMongoTreatmentPlanDocument(treatmentPlanDocumentIncoming);
                
                var mimeType = ByteHandler.GetMimeType(treatmentPlanDocument.FileType);
                //Upload to blob
                var uploaded = await mediaContainer.UploadFileToStorage(ByteHandler.Base64DecodeFileString(treatmentPlanDocumentIncoming.File), treatmentPlanDocument.FileInfoId.ToString(), mimeType);

                await treatmentPlanRepository.AddTreatmentPlanDocument(treatmentPlanDocument, treatmentPlanIdToWriteTo);

                logger.LogInformation($"Successfully Uploaded TreatmentPlanDocument with ID: {treatmentPlanIdToWriteTo}");

            }
        }

        public async Task DeleteTreatmentPlanDocument(string TreatmentPlanDocumentId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:DeleteTreatmentPlanDocument"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanDocumentId, IdType.TreatmentPlan);

                await treatmentPlanRepository.DeleteTreatmentPlanDocument(TreatmentPlanDocumentId);

                logger.LogInformation($"Successfully deleted TreatmentPlanDocument with ID: {TreatmentPlanDocumentId}");

            }
        }

        public async Task<List<TreatmentPlanOutgoing>> GetTreatmentPlans(string OrganisationId, string ServiceproviderId, string? CustomerId)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:GetAllTreatmentPlans"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(OrganisationId, IdType.Organisation);

                var mongoTreatmentPlans = await treatmentPlanRepository.GetAllTreatmentPlans(OrganisationId, ServiceproviderId);

                logger.LogInformation($"Received {mongoTreatmentPlans.Count} treatment plans from db");

                var outgoingTreatmentPlans = TreatmentPlanConverter.ConvertToOutgoingTreatmentPlanList(mongoTreatmentPlans);

                logger.LogInformation("Converted treatment plans to outgoing successfully");

                return outgoingTreatmentPlans;

            }
        }

        public async Task UpdateTreatment(string TreatmentPlanId, TreatmentIncoming treatmentIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:UpdateTreatment"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(TreatmentPlanId, IdType.TreatmentPlan);
                DataValidation.ValidateObjectId(treatmentIncoming.TreatmentId, IdType.TreatmentPlan);

                var mongoTreatment = TreatmentPlanConverter.ConvertToMongoTreatment(treatmentIncoming);

                logger.LogInformation("Converted to mongo treatment successfully");

                await treatmentPlanRepository.UpdateTreatment(TreatmentPlanId, mongoTreatment);

                logger.LogInformation($"Updated treatment with id:{treatmentIncoming.TreatmentId} successfully");
            }
        }

        public async Task UpdateTreatmentPlan(TreatmentPlanIncoming treatmentPlanIncoming)
        {
            using (logger.BeginScope("Method: {Method}", "TreatmentPlanService:UpdateTreatmentPlan"))
            using (logger.BeginScope(NambaDoctorContext.TraceContextValues))
            {
                DataValidation.ValidateObjectId(treatmentPlanIncoming.TreatmentPlanId, IdType.TreatmentPlan);

                //Here spName and custName are no longer required as we dont update that
                var mongoTreatmentPlan = TreatmentPlanConverter.ConvertToMongoTreatmentPlan(treatmentPlanIncoming, null, null);

                logger.LogInformation("Converted to mongo treatment plan successfully");

                await treatmentPlanRepository.UpsertTreatmentPlan(mongoTreatmentPlan);

                logger.LogInformation($"Updated treatment plan with id:{treatmentPlanIncoming.TreatmentPlanId} successfully");
            }
        }

        private async Task<string> CreateNewBlankTreatmentPlan(Mongo.Appointment appointment)
        {
            //Create new treatment plan with appointment details
            
            var mongoTreatmentPlan = TreatmentPlanConverter.GetNewMongoTreatmentPlanWithBlankData(
                appointment
            );
                    
            logger.LogInformation("Constructed mongo treatment plan obj");

            await treatmentPlanRepository.Add(mongoTreatmentPlan);

            logger.LogInformation($"Created new treatment plan with id: {mongoTreatmentPlan.TreatmentPlanId}");
            
            return mongoTreatmentPlan.TreatmentPlanId.ToString();
        }

        private void FilterTreatmentPlans(ref List<ProviderClientOutgoing.TreatmentOutgoing> treatments)
        {
            treatments.RemoveAll(treatment =>
                treatment.Status == Mongo.TreatmentStatus.Cancelled.ToString() ||
                treatment.Status == Mongo.TreatmentStatus.Done.ToString() ||
                treatment.Status == Mongo.TreatmentStatus.BookedAppointment.ToString()); 
        }
    }
}
