using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public class QueueMessageParser : IQueueMessageParser
    {
        public FinanceQueueMessage? Parse(string queueMessage, ILogger log)
        {
            clientEmail = string.Empty;
            month = 0;
            year = 0;
            eventType = default;

            try
            {
                var msg = System.Text.Json.JsonDocument.Parse(queueMessage).RootElement;

                if (!msg.TryGetProperty("Record", out var record))
                {
                    log.LogError("Message does not contain 'Record' property.");
                    return null;
                }

                var clientEmail = record.TryGetProperty("Email", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;
                if (string.IsNullOrWhiteSpace(clientEmail))
                {
                    log.LogError("No email found in message.");
                    return null;
                }

                var month = record.TryGetProperty("Month", out var monthProp) ? monthProp.GetInt32() : 0;
                var year = record.TryGetProperty("Year", out var yearProp) ? yearProp.GetInt32() : 0;

                var eventTypeString = msg.TryGetProperty("Event", out var eventProp) ? eventProp.GetString() ?? string.Empty : string.Empty;
                if (string.IsNullOrWhiteSpace(eventTypeString))
                {
                    log.LogWarning("No 'Event' property found in message.");
                    return null;
                }

                if (!Enum.TryParse(eventTypeString, out FinanceEventType eventType))
                {
                    log.LogWarning("Unknown event type: {EventType}", eventTypeString);
                    return null;
                }

                return new FinanceQueueMessage
                {
                    Email = clientEmail,
                    Month = month,
                    Year = year,
                    EventType = eventType
                };

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to parse queue message.");
                return null;
            }
        }
    }
}
