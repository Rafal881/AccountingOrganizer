using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public class FinanceEmailContentProvider : IEmailContentProvider
    {
        public EmailContent? GetContent(FinanceEventType eventType, int month, int year, ILogger log)
        {
            return eventType switch
            {
                FinanceEventType.NewFinancialRecordCreated => new EmailContent
                {
                    Subject = $"Dane Podatkowe {month}/{year}",
                    Body = $"Szanowni Państwo, nowe dane podatkowe za okres {month}/{year} zostały opublikowane."
                },
                FinanceEventType.FinancialRecordUpdated => new EmailContent
                {
                    Subject = $"Dane Podatkowe {month}/{year} - aktualizacja",
                    Body = $"Szanowni Państwo, dane podatkowe za okres {month}/{year} zostały zaktualizowane."
                },
                _ => LogUnknownEvent(FinanceEventType.Unknown, log)
            };
        }

        private static EmailContent? LogUnknownEvent(FinanceEventType eventType, ILogger log)
        {
            log.LogWarning("Unknown event type: {eventType}", eventType);
            return null;
        }
    }
}
