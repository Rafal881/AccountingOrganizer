namespace ClientOrganizer.API.Models.Dtos;

public record FinanceServiceResult(FinanceServiceError Error, FinancialRecordReadDto? Record);
