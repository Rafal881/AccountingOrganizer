using ClientOrganizer.API.Models.Dtos;

namespace ClientOrganizer.API.Services;

public record FinanceServiceResult(FinanceServiceError Error, FinancialRecordReadDto? Record);
