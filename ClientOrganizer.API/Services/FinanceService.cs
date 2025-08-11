using AutoMapper;
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
    private readonly IMapper _mapper;

    public FinanceService(ClientOrganizerDbContext db, FinanceMessageService messageService, IMapper mapper)
    {
        dbContext = db;
        _messageService = messageService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FinancialRecordReadDto>> GetAllForClientAsync(int clientId)
    {
        var records = await _db.FinancialData
            .Where(f => f.ClientId == clientId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<FinancialRecordReadDto>>(records);
    }

    public async Task<FinancialRecordReadDto?> GetByIdAsync(int id)
    {
        var record = await _db.FinancialData.FindAsync(id);
        return record is null ? null : _mapper.Map<FinancialRecordReadDto>(record);
    }

    public async Task<FinancialRecordReadDto?> GetByClientMonthYearAsync(int clientId, int month, int year)
    {
        var record = await _db.FinancialData
            .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == month && f.Year == year);
        return record is null ? null : _mapper.Map<FinancialRecordReadDto>(record);
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

        var record = _mapper.Map<FinancialData>(createDto);
        record.ClientId = clientId;
        record.Client = client;

        dbContext.FinancialData.Add(record);
        await dbContext.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

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

        _mapper.Map(updateDto, record);

        await dbContext.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

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

