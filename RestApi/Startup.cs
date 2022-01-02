using DataLayer;
using DataModel.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiddleWare.Interfaces;
using MiddleWare.Services;
using RestApi.Middlewares;
using System;
using System.IO;

namespace NambaDoctorWebApi
{
    public class Startup
    {
        public const string FIREBASE_PROJECT_ID = "ds-connect";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            ReadSecretsFromMount();
            */

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.Authority = $"https://securetoken.google.com/{FIREBASE_PROJECT_ID}";
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = $"https://securetoken.google.com/ds-connect{FIREBASE_PROJECT_ID}",
                     ValidateAudience = true,
                     ValidAudience = $"{FIREBASE_PROJECT_ID}",
                     ValidateLifetime = true
                 };
             });


            services.AddApplicationInsightsTelemetry();

            //Init Logging
            services.AddScoped<NambaDoctorContext>();

            //Services
            services.AddScoped<IOrganisationService, OrganisationService>();
            services.AddScoped<IServiceProviderService, ServiceProviderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAppointmentService, AppointmentService>();

            //Init datalayer with telemetry
            services.AddSingleton<IMongoDbDataLayer, BaseMongoDBDataLayer>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NambaDoctorWebApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                    new string[] {}}
                });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:3000")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
        }

        public void ReadSecretsFromMount()
        {
            const string mountPath = "/mnt/secrets-store";
            Console.WriteLine($"Processing: {mountPath}");
            try
            {
                var allSecretFiles = Directory.GetFiles(mountPath);
                bool isppe = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "PPE";
                if (!isppe)
                {
                    foreach (var f in allSecretFiles)
                    {
                        switch (f)
                        {
                            case "/mnt/secrets-store/twilio-api-secret":
                                ConnectionConfiguration.TwilioApiSecret = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/twilio-api-key":
                                ConnectionConfiguration.TwilioApiKey = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/twilio-account-sid":
                                ConnectionConfiguration.TwilioAccountSid = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/mongo-connection-string":
                                ConnectionConfiguration.MongoConnectionString = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/fcm-server-key":
                                ConnectionConfiguration.FcmServerKey = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/fcm-sender-id":
                                ConnectionConfiguration.FcmSenderId = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/blob-storage-connection-string":
                                ConnectionConfiguration.BlobStorageConnectionString = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/apn-bundle-id":
                                ConnectionConfiguration.ApnBundleId = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/apn-p8-private-key-id":
                                ConnectionConfiguration.ApnP8PrivateKeyId = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/apn-team-id":
                                ConnectionConfiguration.ApnTeamId = File.ReadAllText(f);
                                break;
                            case "/mnt/secrets-store/apn-server-type":
                                ConnectionConfiguration.ApnsServiceType = int.Parse(File.ReadAllText(f));
                                break;
                            default: break;
                        }
                        Console.WriteLine($"Secret '{f}' has value '{File.ReadAllText(f)}'");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"No mounted secrets {e.Message}");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NambaDoctorWebApi v1"));
            }

            app.UseLogContextSet();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseUserContextSet();

            app.UseExceptionLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
