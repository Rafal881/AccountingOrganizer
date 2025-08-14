using ClientOrganizer.API.Application.Services;
using ClientOrganizer.API.Models.Dtos;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClientOrganizer.API.Endpoints;

public static class Finance
{
    public static void RegisterFinanceEndpoints(this WebApplication app)
    {
        // Get all financial records for a given client
        app.MapGet("/clients/{clientId}/finance/all", async (int clientId, IFinanceService service, int page = 1, int pageSize = 10) =>
        {
            var records = await service.GetAllForClientAsync(clientId, page, pageSize);
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
        app.MapPost("/clients/{clientId}/finance", async (int clientId, FinancialRecordCreateDto createDto, 
            IFinanceService service, 
            IValidator<FinancialRecordCreateDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(
                    validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

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
        app.MapPut("/clients/{clientId}/finance", async (int clientId, FinancialRecordUpdateDto updateDto, 
            IFinanceService service, 
            IValidator < FinancialRecordUpdateDto > validator) =>
        {
            var validationResult = await validator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(
                    validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

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

