using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Endpoints
{
    public static class Finance
    {
        public static void RegisterFinanceEndpoints(this WebApplication app)
        {
            // Get all financial records
            app.MapGet("/finance", async (ClientOrganizerDbContext db) =>
            {
                var records = await db.FinancialData
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

                return Results.Ok(records);
            })
            .WithName("GetFinanceRecords")
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
            app.MapPost("/clients/{clientId}/finance", async (int clientId, FinancialRecordDto recordDto, ClientOrganizerDbContext db) =>
            {
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
                return Results.Created($"/finance/{record.Id}", recordDto);
            })
            .WithName("CreateClientFinanceRecord")
            .WithTags("Finance");

            // Update a financial record for a client
            app.MapPut("/clients/{clientId}/finance", async (int clientId, FinancialRecordDto recordDto, ClientOrganizerDbContext db) =>
            {
                var record = await db.FinancialData
                    .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == recordDto.Month && f.Year == recordDto.Year);

                if (record is null)
                    return Results.NotFound();

                record.IncomeTax = recordDto.IncomeTax;
                record.Vat = recordDto.Vat;
                record.InsuranceAmount = recordDto.InsuranceAmount;

                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithName("UpdateClientFinanceRecord")
            .WithTags("Finance");
        }
    }
}
