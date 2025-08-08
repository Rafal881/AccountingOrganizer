using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public class FinanceEmailContentProvider : IEmailContentProvider
    {
        public EmailContent? GetContent(string eventType, int month, int year, ILogger log)
        {
            return eventType switch
            {
                "NewFinancialRecordCreated" => new EmailContent
                {
                    Subject = $"Dane Podatkowe {month}/{year}",
                    Body = $"Szanowni Państwo, nowe dane podatkowe za okres {month}/{year} zostały opublikowane."
                },
                "FinancialRecordUpdated" => new EmailContent
                {
                    Subject = $"Dane Podatkowe {month}/{year} - aktualizacja",
                    Body = $"Szanowni Państwo, dane podatkowe za okres {month}/{year} zostały zaktualizowane."
                },
                _ => LogUnknownEvent(eventType, log)
            };
        }

        private EmailContent? LogUnknownEvent(string eventType, ILogger log)
        {
            log.LogWarning("Unknown event type: {eventType}", eventType);
            return null;
        }
    }
}
