using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Endpoints
{
    public static class Finance
    {
        public static void RegisterFinanceEndpoints(this WebApplication app)
        {
            // Get all financial records for a given client
            app.MapGet("/clients/{clientId}/finance/all", async (int clientId, ClientOrganizerDbContext db) =>
            {
                var records = await db.FinancialData
                    .Where(f => f.ClientId == clientId)
                    .Select(f => new FinancialRecordDto
                    {
                        Id = f.Id,
                        ClientId = f.ClientId,
                        Month = f.Month,
                        Year = f.Year,
                        IncomeTax = f.IncomeTax,
                        Vat = f.Vat,
                        InsuranceAmount = f.InsuranceAmount
                    })
                    .ToListAsync();

                return records.Count != 0 ? Results.Ok(records) : Results.NotFound();
            })
            .WithName("GetAllFinanceRecordsForClient")
            .WithTags("Finance");

            // Get a specific financial record by its ID
            app.MapGet("/finance/{id}", async (int id, ClientOrganizerDbContext db) =>
            {
                var record = await db.FinancialData
                    .Where(f => f.Id == id)
                    .Select(f => new FinancialRecordDto
                    {
                        Id = f.Id,
                        ClientId = f.ClientId,
                        Month = f.Month,
                        Year = f.Year,
                        IncomeTax = f.IncomeTax,
                        Vat = f.Vat,
                        InsuranceAmount = f.InsuranceAmount
                    })
                    .FirstOrDefaultAsync();

                return record is not null ? Results.Ok(record) : Results.NotFound();
            })
            .WithName("GetFinanceRecordById")
            .WithTags("Finance");

            // Get financial info for a client filtered by month and year
            app.MapGet("/clients/{clientId}/finance", async (int clientId, int month, int year, ClientOrganizerDbContext db) =>
            {
                var record = await db.FinancialData
                    .Where(f => f.ClientId == clientId && f.Month == month && f.Year == year)
                    .Select(f => new FinancialRecordDto
                    {
                        Id = f.Id,
                        ClientId = f.ClientId,
                        Month = f.Month,
                        Year = f.Year,
                        IncomeTax = f.IncomeTax,
                        Vat = f.Vat,
                        InsuranceAmount = f.InsuranceAmount
                    })
                    .FirstOrDefaultAsync();

                return record is not null ? Results.Ok(record) : Results.NotFound();
            })
            .WithName("GetClientFinanceByMonthYear")
            .WithTags("Finance");

            // Create a new financial record for a client
            app.MapPost("/clients/{clientId}/finance", async (int clientId, FinancialRecordDto recordDto, ClientOrganizerDbContext db, FinanceMessageService messageService) =>
            {
                var exists = await db.FinancialData
                    .AnyAsync(f => f.ClientId == clientId && f.Month == recordDto.Month && f.Year == recordDto.Year);

                if (exists)
                    return Results.Conflict($"Financial record for client {clientId} in {recordDto.Month}/{recordDto.Year} already exists.");

                var record = new FinancialData
                {
                    ClientId = clientId,
                    Month = recordDto.Month,
                    Year = recordDto.Year,
                    IncomeTax = recordDto.IncomeTax,
                    Vat = recordDto.Vat,
                    InsuranceAmount = recordDto.InsuranceAmount
                };

                db.FinancialData.Add(record);
                await db.SaveChangesAsync();

                recordDto.Id = record.Id;
                recordDto.ClientId = clientId;

                await messageService.SendFinancialRecordCreatedAsync(recordDto);

                return Results.Created($"/finance/{record.Id}", recordDto);
            })
            .WithName("CreateClientFinanceRecord")
            .WithTags("Finance");

            // Update a financial record for a client
            app.MapPut("/clients/{clientId}/finance", async (int clientId, FinancialRecordUpdateDto updateDto, ClientOrganizerDbContext db, FinanceMessageService messageService) =>
            {
                if (updateDto is null ||
                    updateDto.IncomeTax is null &&
                    updateDto.Vat is null &&
                    updateDto.InsuranceAmount is null)
                {
                    return Results.BadRequest("At least one property must be provided for update.");
                }

                if (updateDto.Month is null || updateDto.Year is null)
                    return Results.BadRequest("Month and Year are required to identify the record.");

                var record = await db.FinancialData
                    .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == updateDto.Month && f.Year == updateDto.Year);

                if (record is null)
                    return Results.NotFound();

                if (updateDto.IncomeTax is not null) record.IncomeTax = updateDto.IncomeTax.Value;
                if (updateDto.Vat is not null) record.Vat = updateDto.Vat.Value;
                if (updateDto.InsuranceAmount is not null) record.InsuranceAmount = updateDto.InsuranceAmount.Value;

                await db.SaveChangesAsync();

                var updatedRecordDto = new FinancialRecordDto
                {
                    Id = record.Id,
                    ClientId = record.ClientId,
                    Month = record.Month,
                    Year = record.Year,
                    IncomeTax = record.IncomeTax,
                    Vat = record.Vat,
                    InsuranceAmount = record.InsuranceAmount
                };
                await messageService.SendFinancialRecordUpdatedAsync(updatedRecordDto);

                return Results.NoContent();
            })
            .WithName("UpdateClientFinanceRecord")
            .WithTags("Finance");

            // Delete a financial record by its ID
            app.MapDelete("/finance/{id}", async (int id, ClientOrganizerDbContext db) =>
            {
                var record = await db.FinancialData.FindAsync(id);
                if (record is null)
                    return Results.NotFound();

                db.FinancialData.Remove(record);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("DeleteFinanceRecord")
            .WithTags("Finance");
        }
    }
}
