using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ClientOrganizer.API.Configuration;
using ClientOrganizer.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVaultUri"];

if (string.IsNullOrWhiteSpace(keyVaultUri))
{
    throw new InvalidOperationException("KeyVaultUri is not configured.");
}

TokenCredential cred = builder.Environment.IsDevelopment()
    ? new AzureCliCredential()
    : new DefaultAzureCredential();

var client = new SecretClient(new Uri(keyVaultUri), cred);
builder.Configuration.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());

builder.RegisterServices();

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterClientsEndpoints();
app.RegisterFinanceEndpoints();

app.Run();
