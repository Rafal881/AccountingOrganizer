using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Models.Dtos;

public class FinanceMessageService
{
    private readonly ServiceBusSender _sender;

    public FinanceMessageService(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public async Task SendFinancialRecordCreatedAsync(FinancialRecordDto dto, string email)
    {

        var messageBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            Event = "NewFinancialRecordCreated",
            Record = new
            {
                dto.Id,
                dto.ClientId,
                dto.Month,
                dto.Year,
                dto.IncomeTax,
                dto.Vat,
                dto.InsuranceAmount,
                Email = email
            }
        });
        var message = new ServiceBusMessage(messageBody);
        await _sender.SendMessageAsync(message);
    }

    public async Task SendFinancialRecordUpdatedAsync(FinancialRecordDto dto, string email)
    {
        var messageBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            Event = "FinancialRecordUpdated",
            Record = new
            {
                dto.Id,
                dto.ClientId,
                dto.Month,
                dto.Year,
                dto.IncomeTax,
                dto.Vat,
                dto.InsuranceAmount,
                Email = email
            }
        });
        var message = new ServiceBusMessage(messageBody);
        await _sender.SendMessageAsync(message);
    }
}
