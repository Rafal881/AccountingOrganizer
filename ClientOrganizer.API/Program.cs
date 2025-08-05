using ClientOrganizer.API.Configuration;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Endpoints;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterClientsEndpoints();
app.RegisterFinanceEndpoints();

app.Run();
