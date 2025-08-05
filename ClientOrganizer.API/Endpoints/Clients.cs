using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models;
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
                    .Select(c => new ClientDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Address = c.Address,
                        NipNb = c.NipNb,
                        Email = c.Email
                    })
                    .ToListAsync();

                return Results.Ok(clients);
            })
            .WithName("GetClients")
            .WithTags("Clients");

            // Get client by id
            app.MapGet("/clients/{id}", async (int id, ClientOrganizerDbContext db) =>
            {
                var client = await db.Clients
                    .Where(c => c.Id == id)
                    .Select(c => new ClientDto
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
            app.MapPost("/clients", async (ClientDto clientDto, ClientOrganizerDbContext db) =>
            {
                var client = new Client
                {
                    Name = clientDto.Name,
                    Address = clientDto.Address,
                    NipNb = clientDto.NipNb,
                    Email = clientDto.Email
                };

                db.Clients.Add(client);
                await db.SaveChangesAsync();

                clientDto.Id = client.Id;
                return Results.Created($"/clients/{client.Id}", clientDto);
            })
            .WithName("CreateClient")
            .WithTags("Clients");

            // Update client
            app.MapPut("/clients/{id}", async (int id, ClientDto clientDto, ClientOrganizerDbContext db) =>
            {
                var client = await db.Clients.FindAsync(id);
                if (client is null)
                    return Results.NotFound();

                client.Name = clientDto.Name;
                client.Address = clientDto.Address;
                client.NipNb = clientDto.NipNb;
                client.Email = clientDto.Email;

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
