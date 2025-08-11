using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using ClientOrganizer.API.Services.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Services;

public class FinanceService : IFinanceService
{
    private readonly ClientOrganizerDbContext dbContext;
    private readonly FinanceMessageService _messageService;

    public FinanceService(ClientOrganizerDbContext db, FinanceMessageService messageService)
    {
        dbContext = db;
        _messageService = messageService;
    }

    public async Task<IEnumerable<FinancialRecordReadDto>> GetAllForClientAsync(int clientId)
    {
        return await dbContext.FinancialData
            .Where(f => f.ClientId == clientId)
            .Select(f => new FinancialRecordReadDto
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
    }

    public async Task<FinancialRecordReadDto?> GetByIdAsync(int id)
    {
        return await dbContext.FinancialData
            .Where(f => f.Id == id)
            .Select(f => new FinancialRecordReadDto
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
    }

    public async Task<FinancialRecordReadDto?> GetByClientMonthYearAsync(int clientId, int month, int year)
    {
        return await dbContext.FinancialData
            .Where(f => f.ClientId == clientId && f.Month == month && f.Year == year)
            .Select(f => new FinancialRecordReadDto
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
    }

    public async Task<FinanceServiceResult> CreateAsync(int clientId, FinancialRecordCreateDto createDto)
    {
        var exists = await dbContext.FinancialData
            .AnyAsync(f => f.ClientId == clientId && f.Month == createDto.Month && f.Year == createDto.Year);

        if (exists)
            return new FinanceServiceResult(FinanceServiceError.Conflict, null);

        var client = await dbContext.Clients.FindAsync(clientId);
        if (client is null)
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);

        var record = new FinancialData
        {
            ClientId = clientId,
            Month = createDto.Month,
            Year = createDto.Year,
            IncomeTax = createDto.IncomeTax,
            Vat = createDto.Vat,
            InsuranceAmount = createDto.InsuranceAmount,
            Client = client
        };

        dbContext.FinancialData.Add(record);
        await dbContext.SaveChangesAsync();

        var recordDto = new FinancialRecordReadDto
        {
            Id = record.Id,
            ClientId = record.ClientId,
            Month = record.Month,
            Year = record.Year,
            IncomeTax = record.IncomeTax,
            Vat = record.Vat,
            InsuranceAmount = record.InsuranceAmount
        };

        await _messageService.SendFinancialRecordCreatedAsync(recordDto, client.Email);

        return new FinanceServiceResult(FinanceServiceError.None, recordDto);
    }

    public async Task<FinanceServiceResult> UpdateAsync(int clientId, FinancialRecordUpdateDto updateDto)
    {
        var record = await dbContext.FinancialData
            .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == updateDto.Month && f.Year == updateDto.Year);

        if (record is null)
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);

        var client = await dbContext.Clients.FindAsync(clientId);
        if (client is null)
        {
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);
        }

        if (updateDto.IncomeTax is not null) record.IncomeTax = updateDto.IncomeTax.Value;
        if (updateDto.Vat is not null) record.Vat = updateDto.Vat.Value;
        if (updateDto.InsuranceAmount is not null) record.InsuranceAmount = updateDto.InsuranceAmount.Value;

        await dbContext.SaveChangesAsync();

        var recordDto = new FinancialRecordReadDto
        {
            Id = record.Id,
            ClientId = record.ClientId,
            Month = record.Month,
            Year = record.Year,
            IncomeTax = record.IncomeTax,
            Vat = record.Vat,
            InsuranceAmount = record.InsuranceAmount
        };

        await _messageService.SendFinancialRecordUpdatedAsync(recordDto, client.Email);

        return new FinanceServiceResult(FinanceServiceError.None, recordDto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await dbContext.FinancialData.FindAsync(id);
        if (record is null) return false;

        dbContext.FinancialData.Remove(record);
        await dbContext.SaveChangesAsync();
        return true;
    }
}

