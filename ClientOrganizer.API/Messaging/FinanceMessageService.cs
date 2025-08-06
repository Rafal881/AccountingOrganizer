using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Models.Dtos;

public class FinanceMessageService
{
    private readonly ServiceBusSender _sender;

    public FinanceMessageService(ServiceBusSender sender)
    {
        _sender = sender;
    }

    public async Task SendFinancialRecordCreatedAsync(FinancialRecordDto dto)
    {
        var messageBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            Event = "NewFinancialRecordCreated",
            Record = dto
        });
        var message = new ServiceBusMessage(messageBody);
        await _sender.SendMessageAsync(message);
    }

    public async Task SendFinancialRecordUpdatedAsync(FinancialRecordDto dto)
    {
        var messageBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            Event = "FinancialRecordUpdated",
            Record = dto
        });
        var message = new ServiceBusMessage(messageBody);
        await _sender.SendMessageAsync(message);
    }
}
