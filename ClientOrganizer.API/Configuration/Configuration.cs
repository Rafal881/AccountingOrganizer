using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Services.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Configuration
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            builder.Services
                   .AddEndpointsApiExplorer()
                   .AddSwaggerGen();

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
            builder.Services.AddSingleton<ServiceBusClient>(sp =>
            {
                var connectionString = builder.Configuration["ServiceBus:ConnectionString"];
                return new ServiceBusClient(connectionString);
            });

            builder.Services.AddSingleton<ServiceBusSender>(sp =>
            {
                var client = sp.GetRequiredService<ServiceBusClient>();
                var queueName = builder.Configuration["ServiceBus:QueueName"];
                return client.CreateSender(queueName);
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
