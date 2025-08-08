using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public class QueueMessageParser : IQueueMessageParser
    {
        public bool TryParse(string queueMessage, ILogger log, out string clientEmail, out int month, out int year, out string eventType)
        {
            clientEmail = string.Empty;
            month = 0;
            year = 0;
            eventType = string.Empty;

            try
            {
                var msg = System.Text.Json.JsonDocument.Parse(queueMessage).RootElement;

                if (!msg.TryGetProperty("Record", out var record))
                {
                    log.LogError("Message does not contain 'Record' property.");
                    return false;
                }

                clientEmail = record.TryGetProperty("Email", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;
                if (string.IsNullOrWhiteSpace(clientEmail))
                {
                    log.LogError("No email found in message.");
                    return false;
                }

                month = record.TryGetProperty("Month", out var monthProp) ? monthProp.GetInt32() : 0;
                year = record.TryGetProperty("Year", out var yearProp) ? yearProp.GetInt32() : 0;

                eventType = msg.TryGetProperty("Event", out var eventProp) ? eventProp.GetString() ?? string.Empty : string.Empty;
                if (string.IsNullOrWhiteSpace(eventType))
                {
                    log.LogWarning("No 'Event' property found in message.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to parse queue message.");
                return false;
            }
        }
    }
}
