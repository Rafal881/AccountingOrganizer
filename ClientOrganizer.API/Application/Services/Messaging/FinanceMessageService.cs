using System.Text.Json;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;

namespace ClientOrganizer.API.Application.Services.Messaging
{
    public class FinanceMessageService
    {
        public OutboxMessage CreateFinancialRecordCreatedMessage(FinancialRecordReadDto dto, string email)
            => CreateMessage(FinanceEventType.NewFinancialRecordCreated, dto, email);

        public OutboxMessage CreateFinancialRecordUpdatedMessage(FinancialRecordReadDto dto, string email)
            => CreateMessage(FinanceEventType.FinancialRecordUpdated, dto, email);

        private static OutboxMessage CreateMessage(FinanceEventType eventType, FinancialRecordReadDto dto, string email)
        {
            var payload = JsonSerializer.Serialize(new
            {
                Event = eventType.ToString(),
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

            return OutboxMessage.Create(eventType.ToString(), payload);
        }
    }
}
