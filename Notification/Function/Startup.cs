using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.GenericRepository.Context;
using MongoDB.GenericRepository.Interfaces;
using MongoDB.GenericRepository.Repository;
using MongoDB.GenericRepository.UoW;
using Notification.Mode.SMS;

namespace Notification.Function
{
    public class Startup : FunctionsStartup
    {
        [assembly: FunctionsStartup(typeof(Startup))]
        public override void Configure(IFunctionsHostBuilder builder)
        {
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
        }
    }
}