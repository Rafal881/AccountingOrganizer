using ClientOrganizer.API.Models.Dtos;

namespace ClientOrganizer.API.Application.Services;

public interface IFinanceService
{
    Task<IEnumerable<FinancialRecordReadDto>> GetAllForClientAsync(int clientId, int page, int pageSize);
    Task<FinancialRecordReadDto?> GetByIdAsync(int id);
    Task<FinancialRecordReadDto?> GetByClientMonthYearAsync(int clientId, int month, int year);
    Task<FinanceServiceResult> CreateAsync(int clientId, FinancialRecordCreateDto createDto);
    Task<FinanceServiceResult> UpdateAsync(int clientId, FinancialRecordUpdateDto updateDto);
    Task<bool> DeleteAsync(int id);
}

