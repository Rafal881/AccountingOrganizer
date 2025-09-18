using Azure.Identity;
using ClientOrganizer.FunctionApp;
using ClientOrganizer.FunctionApp.Configuration;
using ClientOrganizer.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SendGrid;

var builder = FunctionsApplication.CreateBuilder(args);

var keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");

if (string.IsNullOrWhiteSpace(keyVaultUri))
{
    throw new InvalidOperationException("KeyVaultUri is not configured.");
}

builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());

var serviceBusConnectionString = builder.Configuration["serviceBusConnectionString"]
    ?? throw new InvalidOperationException("Service Bus connection string not configured.");

builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddServiceBusClient(serviceBusConnectionString);
});

builder.Services.Configure<SendGridOptions>(options =>
{
    options.ApiKey = builder.Configuration["SendGridApiKey"]
        ?? throw new InvalidOperationException("SendGrid ApiKey configuration is missing.");
    options.FromEmail = builder.Configuration["SendGridFromEmail"]
        ?? throw new InvalidOperationException("SendGrid FromEmail configuration is missing.");
});

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<SendGridOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.ApiKey))
    {
        throw new InvalidOperationException("SendGrid ApiKey configuration is missing.");
    }
    return new SendGridClient(options.ApiKey);
});

builder.Services.AddSingleton<IQueueMessageParser, QueueMessageParser>();
builder.Services.AddSingleton<ISendGridMessageFactory, SendGridMessageFactory>();
builder.Services.AddSingleton<IEmailContentProvider, FinanceEmailContentProvider>();

builder.Services.AddScoped<FinanceEmailFunction>();

builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
