using ClientOrganizer.API.Configuration;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Endpoints;
using Microsoft.EntityFrameworkCore;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

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

// Register EF Core DbContext
builder.Services.AddDbContext<ClientOrganizerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.RegisterServices();

// Add Service Bus client and sender to DI
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

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterClientsEndpoints();
app.RegisterFinanceEndpoints();

app.Run();
