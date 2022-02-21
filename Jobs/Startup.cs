using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.GenericRepository.Context;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using MongoDB.GenericRepository.UoW;
using NotificationUtil.Mode.SMS;
using NotificationUtil.Trigger;
using System;

[assembly: FunctionsStartup(typeof(Notification.Function.Startup))]
namespace Notification.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Console.WriteLine("FunctionsStartup called");
            builder.Services.AddScoped<IMongoContext, MongoContext>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<ISmsService, SmsService>();
            var testSms = Environment.GetEnvironmentVariable("sms_test");
            builder.Services.AddScoped<ISmsRepository, SmsRepository>((repository) =>
            {
                return new SmsRepository(testSms != null ? bool.Parse(testSms) : true);
            });

            builder.Services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<INotificationQueueRepository, NotificationQueueRepository>();
            builder.Services.AddScoped<INotificationBroadcast, NotificationBroadcast>();
        }
    }
}