using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Services;
using System.Linq;

namespace ClientOrganizer.API.Endpoints;

public static class Finance
{
    public static void RegisterFinanceEndpoints(this WebApplication app)
    {
        // Get all financial records for a given client
        app.MapGet("/clients/{clientId}/finance/all", async (int clientId, IFinanceService service) =>
        {
            var records = await service.GetAllForClientAsync(clientId);
            return records.Any() ? Results.Ok(records) : Results.NotFound();
        })
        .WithName("GetAllFinanceRecordsForClient")
        .WithTags("Finance");

        // Get a specific financial record by its ID
        app.MapGet("/finance/{id}", async (int id, IFinanceService service) =>
        {
            var record = await service.GetByIdAsync(id);
            return record is not null ? Results.Ok(record) : Results.NotFound();
        })
        .WithName("GetFinanceRecordById")
        .WithTags("Finance");

        // Get financial info for a client filtered by month and year
        app.MapGet("/clients/{clientId}/finance", async (int clientId, int month, int year, IFinanceService service) =>
        {
            var record = await service.GetByClientMonthYearAsync(clientId, month, year);
            return record is not null ? Results.Ok(record) : Results.NotFound();
        })
        .WithName("GetClientFinanceByMonthYear")
        .WithTags("Finance");

        // Create a financial record for a client
        app.MapPost("/clients/{clientId}/finance", async (int clientId, FinancialRecordCreateDto createDto, IFinanceService service) =>
        {
            var result = await service.CreateAsync(clientId, createDto);
            return result.Error switch
            {
                FinanceServiceError.Conflict => Results.Conflict($"Financial record for client {clientId} in {createDto.Month}/{createDto.Year} already exists."),
                FinanceServiceError.NotFound => Results.NotFound($"Client with id {clientId} not found."),
                _ => Results.Created($"/finance/{result.Record!.Id}", result.Record)
            };
        })
        .WithName("CreateClientFinanceRecord")
        .WithTags("Finance");

        // Update a financial record for a client
        app.MapPut("/clients/{clientId}/finance", async (int clientId, FinancialRecordUpdateDto updateDto, IFinanceService service) =>
        {
            if (updateDto is null ||
                updateDto.IncomeTax is null &&
                updateDto.Vat is null &&
                updateDto.InsuranceAmount is null)
            {
                return Results.BadRequest("At least one property must be provided for update.");
            }

            var result = await service.UpdateAsync(clientId, updateDto);
            return result.Error == FinanceServiceError.NotFound ? Results.NotFound() : Results.NoContent();
        })
        .WithName("UpdateClientFinanceRecord")
        .WithTags("Finance");

        // Delete a financial record by its ID
        app.MapDelete("/finance/{id}", async (int id, IFinanceService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteFinanceRecord")
        .WithTags("Finance");
    }
}

