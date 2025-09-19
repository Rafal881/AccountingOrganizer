using AutoMapper;
using ClientOrganizer.API.Application.Services.Messaging;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace ClientOrganizer.API.Application.Services;

public class FinanceService : IFinanceService
{
    private readonly ClientOrganizerDbContext _dbContext;
    private readonly FinanceMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;
    public FinanceService(ClientOrganizerDbContext db, FinanceMessageService messageService, IMapper mapper, HybridCache cache, IUnitOfWork unitOfWork)
    {
        _dbContext = db;
        _messageService = messageService;
        _mapper = mapper;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FinancialRecordReadDto>> GetAllForClientAsync(int clientId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var cacheKey = GetCacheKey(clientId);

        var records = await _cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            var data = await _dbContext.FinancialData
                .Where(f => f.ClientId == clientId)
                .OrderBy(f => f.Id)
                .AsNoTracking()
                .ToListAsync(ct);
            return _mapper.Map<List<FinancialRecordReadDto>>(data);
        });

        return records.Skip((page - 1) * pageSize).Take(pageSize);
    }

    private static string GetCacheKey(int clientId) => $"finance_records_client_{clientId}";

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

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        _dbContext.FinancialData.Add(record);
        await _unitOfWork.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

        if (!await _messageService.SendFinancialRecordCreatedAsync(recordDto, client.Email))
        {
            await transaction.RollbackAsync();
            _dbContext.Entry(record).State = EntityState.Detached;
            return new FinanceServiceResult(FinanceServiceError.ServiceBusError, null);
        }

        await transaction.CommitAsync();

        await _cache.RemoveAsync(GetCacheKey(clientId));

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

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.SaveChangesAsync();

        var recordDto = _mapper.Map<FinancialRecordReadDto>(record);

        if (!await _messageService.SendFinancialRecordUpdatedAsync(recordDto, client.Email))
        {
            await transaction.RollbackAsync();
            await _dbContext.Entry(record).ReloadAsync();
            return new FinanceServiceResult(FinanceServiceError.ServiceBusError, null);
        }

        await transaction.CommitAsync();

        await _cache.RemoveAsync(GetCacheKey(clientId));

        return new FinanceServiceResult(FinanceServiceError.None, recordDto);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _dbContext.FinancialData.FindAsync(id);
        if (record is null) return false;

        _dbContext.FinancialData.Remove(record);
        await _unitOfWork.SaveChangesAsync();

        await _cache.RemoveAsync(GetCacheKey(record.ClientId));

        return true;
    }
}

