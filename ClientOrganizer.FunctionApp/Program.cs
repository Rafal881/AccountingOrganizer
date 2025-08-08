using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;
using ClientOrganizer.FunctionApp;
using ClientOrganizer.FunctionApp.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddServiceBusClient(builder.Configuration["ServiceBusConnection"]);
});

builder.Services.AddSingleton(sp =>
    new SendGridClient(builder.Configuration["SendGridApiKey"]));

builder.Services.AddSingleton<IQueueMessageParser, QueueMessageParser>();
builder.Services.AddSingleton<ISendGridMessageFactory, SendGridMessageFactory>();
builder.Services.AddSingleton<IEmailContentProvider, FinanceEmailContentProvider>();

builder.Services.AddScoped<FinanceEmailFunction>();

builder.ConfigureFunctionsWebApplication();
builder.Build().Run();
