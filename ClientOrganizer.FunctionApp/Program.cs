using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
using ClientOrganizer.FunctionApp;
using ClientOrganizer.FunctionApp.Services;

var builder = FunctionsApplication.CreateBuilder(args);

var serviceBusConnectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString")
    ?? throw new InvalidOperationException("Service Bus connection string not found in environment variables.");

builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddServiceBusClient(serviceBusConnectionString);
});

builder.Services.AddSingleton(sp =>
    new SendGridClient(builder.Configuration["SendGridApiKey"]));

builder.Services.AddSingleton<IQueueMessageParser, QueueMessageParser>();
builder.Services.AddSingleton<ISendGridMessageFactory, SendGridMessageFactory>();
builder.Services.AddSingleton<IEmailContentProvider, FinanceEmailContentProvider>();

builder.Services.AddScoped<FinanceEmailFunction>();

builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
