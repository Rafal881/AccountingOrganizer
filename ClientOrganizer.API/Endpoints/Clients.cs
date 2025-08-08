using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Endpoints
{
    public static class Clients
    {
        public static void RegisterClientsEndpoints(this WebApplication app)
        {
            // Get all clients
            app.MapGet("/clients", async (ClientOrganizerDbContext db) =>
            {
                var clients = await db.Clients
                    .Select(c => new ClientReadDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Address = c.Address,
                        NipNb = c.NipNb,
                        Email = c.Email
                    })
                    .ToListAsync();

                return clients.Count != 0 ? Results.Ok(clients) : Results.NotFound();
            })
            .WithName("GetClients")
            .WithTags("Clients");

            // Get client by id
            app.MapGet("/clients/{id}", async (int id, ClientOrganizerDbContext db) =>
            {
                var client = await db.Clients
                    .Where(c => c.Id == id)
                    .Select(c => new ClientReadDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Address = c.Address,
                        NipNb = c.NipNb,
                        Email = c.Email
                    })
                    .FirstOrDefaultAsync();

                return client is not null ? Results.Ok(client) : Results.NotFound();
            })
            .WithName("GetClientById")
            .WithTags("Clients");

            // Create client
            app.MapPost("/clients", async (ClientCreateDto clientCreateDto, ClientOrganizerDbContext db) =>
            {
                var exists = await db.Clients.AnyAsync(c => c.NipNb == clientCreateDto.NipNb);
                if (exists)
                    return Results.Conflict($"Client with NipNb '{clientCreateDto.NipNb}' already exists.");

                var client = new Client
                {
                    Name = clientCreateDto.Name,
                    Address = clientCreateDto.Address,
                    NipNb = clientCreateDto.NipNb,
                    Email = clientCreateDto.Email
                };

                db.Clients.Add(client);
                await db.SaveChangesAsync();

                var clientDto = new ClientReadDto
                {
                    Id = client.Id,
                    Name = client.Name,
                    Address = client.Address,
                    NipNb = client.NipNb,
                    Email = client.Email
                };

                return Results.Created($"/clients/{client.Id}", clientDto);
            })
            .WithName("CreateClient")
            .WithTags("Clients");

            // Update client
            app.MapPut("/clients/{id}", async (int id, ClientUpdateDto updateDto, ClientOrganizerDbContext db) =>
            {
                if (updateDto is null ||
                    updateDto.Name is null &&
                    updateDto.Address is null &&
                    updateDto.NipNb is null &&
                    updateDto.Email is null)
                {
                    return Results.BadRequest("At least one property must be provided for update.");
                }

                var client = await db.Clients.FindAsync(id);
                if (client is null)
                    return Results.NotFound();

                if (updateDto.Name is not null) client.Name = updateDto.Name;
                if (updateDto.Address is not null) client.Address = updateDto.Address;
                if (updateDto.NipNb is not null) client.NipNb = updateDto.NipNb;
                if (updateDto.Email is not null) client.Email = updateDto.Email;

                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("UpdateClient")
            .WithTags("Clients");

            // Delete client
            app.MapDelete("/clients/{id}", async (int id, ClientOrganizerDbContext db) =>
            {
                var client = await db.Clients.FindAsync(id);
                if (client is null)
                    return Results.NotFound();

                db.Clients.Remove(client);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteClient")
            .WithTags("Clients");
        }
    }
}
