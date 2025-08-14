using ClientOrganizer.API.Application.Services;
using ClientOrganizer.API.Models.Dtos;
using FluentValidation;

namespace ClientOrganizer.API.Endpoints;

public static class Clients
{
    public static void RegisterClientsEndpoints(this WebApplication app)
    {
        // Get all clients
        app.MapGet("/clients", async (IClientService service, int page = 1, int pageSize = 10) =>
        {
            var clients = await service.GetClientsAsync(page, pageSize);
            return clients.Any() ? Results.Ok(clients) : Results.NotFound();
        })
        .WithName("GetClients")
        .WithTags("Clients");

        // Get client by id
        app.MapGet("/clients/{id}", async (int id, IClientService service) =>
        {
            var client = await service.GetClientByIdAsync(id);
            return client is not null ? Results.Ok(client) : Results.NotFound();
        })
        .WithName("GetClientById")
        .WithTags("Clients");

        // Create client
        app.MapPost("/clients", async (ClientCreateDto clientCreateDto, IClientService service, IValidator<ClientCreateDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(clientCreateDto);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(
                    validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

            var created = await service.CreateClientAsync(clientCreateDto);
            if (created is null)
                return Results.Conflict($"Client with NipNb '{clientCreateDto.NipNb}' already exists.");

            return Results.Created($"/clients/{created.Id}", created);
        })
        .WithName("CreateClient")
        .WithTags("Clients");

        // Update client
        app.MapPut("/clients/{id}", async (int id, ClientUpdateDto updateDto, IClientService service, IValidator<ClientUpdateDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(
                    validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

            var updated = await service.UpdateClientAsync(id, updateDto);
            return updated ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateClient")
        .WithTags("Clients");

        // Delete client
        app.MapDelete("/clients/{id}", async (int id, IClientService service) =>
        {
            var deleted = await service.DeleteClientAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteClient")
        .WithTags("Clients");
    }
}
