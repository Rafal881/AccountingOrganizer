using AutoMapper;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using ClientOrganizer.API.Services.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Services;

public class FinanceService : IFinanceService
{
    private readonly ClientOrganizerDbContext _dbContext;
    private readonly FinanceMessageService _messageService;
    private readonly IMapper _mapper;

    public FinanceService(ClientOrganizerDbContext db, FinanceMessageService messageService, IMapper mapper)
    {
        _dbContext = db;
        _messageService = messageService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FinancialRecordReadDto>> GetAllForClientAsync(int clientId)
    {
        var records = await _dbContext.FinancialData
            .Where(f => f.ClientId == clientId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<FinancialRecordReadDto>>(records);
    }

    public async Task<FinancialRecordReadDto?> GetByIdAsync(int id)
    {
        var record = await _dbContext.FinancialData
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);
        return record is null ? null : _mapper.Map<FinancialRecordReadDto>(record);
    }

    public async Task<FinancialRecordReadDto?> GetByClientMonthYearAsync(int clientId, int month, int year)
    {
        var record = await _dbContext.FinancialData
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == month && f.Year == year);
        return record is null ? null : _mapper.Map<FinancialRecordReadDto>(record);
    }

    public async Task<FinanceServiceResult> CreateAsync(int clientId, FinancialRecordCreateDto createDto)
    {
        var exists = await _dbContext.FinancialData
            .AnyAsync(f => f.ClientId == clientId && f.Month == createDto.Month && f.Year == createDto.Year);

        if (exists)
            return new FinanceServiceResult(FinanceServiceError.Conflict, null);

        var client = await _dbContext.Clients.FindAsync(clientId);
        if (client is null)
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);

        var record = _mapper.Map<FinancialData>(createDto);
        record.ClientId = clientId;
        record.Client = client;

        _dbContext.FinancialData.Add(record);
        await _dbContext.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

        await _messageService.SendFinancialRecordCreatedAsync(recordDto, client.Email);

        return new FinanceServiceResult(FinanceServiceError.None, recordDto);
    }

    public async Task<FinanceServiceResult> UpdateAsync(int clientId, FinancialRecordUpdateDto updateDto)
    {
        var record = await _dbContext.FinancialData
            .FirstOrDefaultAsync(f => f.ClientId == clientId && f.Month == updateDto.Month && f.Year == updateDto.Year);

        if (record is null)
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);

        var client = await _dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == clientId);
        if (client is null)
        {
            return new FinanceServiceResult(FinanceServiceError.NotFound, null);
        }

        _mapper.Map(updateDto, record);

        await _dbContext.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

        await _messageService.SendFinancialRecordUpdatedAsync(recordDto, client.Email);

        return new FinanceServiceResult(FinanceServiceError.None, recordDto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _dbContext.FinancialData.FindAsync(id);
        if (record is null) return false;

        _dbContext.FinancialData.Remove(record);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

