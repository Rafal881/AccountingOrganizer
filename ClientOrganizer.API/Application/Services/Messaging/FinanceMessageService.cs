using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Models.Dtos;

namespace ClientOrganizer.API.Application.Services.Messaging
{
    public class FinanceMessageService
    {
        private readonly ServiceBusSender _sender;
        public FinanceMessageService(ServiceBusSender sender)
        {
            _sender = sender;
        }

        public async Task<bool> SendFinancialRecordCreatedAsync(FinancialRecordReadDto dto, string email)
        {
            try
            {
            var messageBody = System.Text.Json.JsonSerializer.Serialize(new
                {
                    Event = FinanceEventType.NewFinancialRecordCreated.ToString(),
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendFinancialRecordUpdatedAsync(FinancialRecordReadDto dto, string email)
        {
            try
            {
                var messageBody = System.Text.Json.JsonSerializer.Serialize(new
                {
                    Event = FinanceEventType.FinancialRecordUpdated.ToString(),
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
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}