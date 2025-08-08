using ClientOrganizer.API.Configuration;
using ClientOrganizer.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();

var app = builder.Build();

app.RegisterMiddlewares();

app.RegisterClientsEndpoints();
app.RegisterFinanceEndpoints();

app.Run();
