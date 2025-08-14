using AutoMapper;
using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Application.Mappings;
using ClientOrganizer.API.Application.Services;
using ClientOrganizer.API.Application.Services.Messaging;
using ClientOrganizer.API.Application.Validators;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ClientOrganizer.API.Configuration
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ClientProfile>();
                cfg.AddProfile<FinanceProfile>();
            });

            var mapperConfigExpression = new MapperConfigurationExpression();
            mapperConfigExpression.AddProfile<ClientProfile>();
            mapperConfigExpression.AddProfile<FinanceProfile>();


            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IFinanceService, FinanceService>();

            builder.Services.AddScoped<IValidator<ClientCreateDto>, ClientCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ClientUpdateDto>, ClientUpdateDtoValidator>();
            builder.Services.AddScoped<IValidator<FinancialRecordCreateDto>, FinancialRecordCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<FinancialRecordUpdateDto>, FinancialRecordUpdateDtoValidator>();


            builder.Services.AddMemoryCache();
            builder.Services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024 * 10; // 10MB
                options.MaximumKeyLength = 512;

                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromMinutes(30)
                };
            });

            // EF Core DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var sqlPassword = Environment.GetEnvironmentVariable("sqlPassword");

            if (!string.IsNullOrWhiteSpace(sqlPassword))
            {
                connectionString += $"Password={sqlPassword};";
            }
            else
            {
                throw new InvalidOperationException("SQL password not found in environment variables.");
            }

            builder.Services.AddDbContext<ClientOrganizerDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Bus client and bus sender
            builder.Services.AddOptions<ServiceBusOptions>()
                .Bind(builder.Configuration.GetSection("ServiceBus"))
                .PostConfigure(options =>
                {
                    var serviceBusConnectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
                    if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
                    {
                        throw new InvalidOperationException("Service Bus connection string not found in environment variables.");
                    }
                    options.ConnectionString = serviceBusConnectionString;
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddSingleton<ServiceBusClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                return new ServiceBusClient(options.ConnectionString);
            });

            builder.Services.AddSingleton<ServiceBusSender>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                return client.CreateSender(options.QueueName);
            });

            builder.Services.AddScoped<FinanceMessageService>();
        }

        public static void RegisterMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger()
                   .UseSwaggerUI();
            }

            app.UseHttpsRedirection();
        }
    }
}
