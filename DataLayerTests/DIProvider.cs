using DataModel.Shared;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.GenericRepository.Context;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using MongoDB.GenericRepository.UoW;
using ND.DataLayer.Utils.BlobStorage;
using System;

namespace LayerTests
{
    public class DIProvider
    {
        public IServiceProvider ServiceProvider { get; set; }
        public DIProvider()
        {
            var services = new ServiceCollection();

            //Init Logging
            services.AddScoped<NambaDoctorContext>();

            //Init datalayer with telemetry
            services.AddSingleton<IMediaContainer, MediaContainer>();

            //Datalayer dependencies
            services.AddScoped<IMongoContext, MongoContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            services.AddScoped<ITreatmentPlanRepository, TreatmentPlanRepository>();
            services.AddScoped<INotificationUserConfigurationRepository, NotificationUserConfigurationRepository>();
            services.AddScoped<INotificationQueueRepository, NotificationQueueRepository>();
            services.AddScoped<ISettingsConfigurationRepository, SettingsConfigurationRepository>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public T? GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
