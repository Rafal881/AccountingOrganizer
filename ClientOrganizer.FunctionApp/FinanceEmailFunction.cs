using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ClientOrganizer.FunctionApp
{
    public class FinanceEmailFunction
    {
        private readonly SendGridClient _sendGridClient;
        private readonly string _fromEmail = Environment.GetEnvironmentVariable("SendGridFromEmail")!;

        public FinanceEmailFunction(SendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient;
        }

        [Function("FinanceEmailFunction")]
        public async Task Run(
            [ServiceBusTrigger("%FinanceQueueName%", Connection = "ServiceBusConnection")] string queueMessage,
            ILogger log)
        {
            var msg = System.Text.Json.JsonDocument.Parse(queueMessage).RootElement;

            if (!msg.TryGetProperty("Record", out var record))
            {
                log.LogError("Message does not contain 'Record' property.");
                return;
            }

            var clientEmail = record.TryGetProperty("Email", out var emailProp) ? emailProp.GetString() : null;
            if (string.IsNullOrWhiteSpace(clientEmail))
            {
                log.LogError("No email found in message.");
                return;
            }

            var month = record.TryGetProperty("Month", out var monthProp) ? monthProp.GetInt32() : 0;
            var year = record.TryGetProperty("Year", out var yearProp) ? yearProp.GetInt32() : 0;

            var subject = string.Empty;
            var body = string.Empty;

            if (msg.TryGetProperty("Event", out var eventProp))
            {
                var eventType = eventProp.GetString();
                if (eventType == "NewFinancialRecordCreated")
                {
                    subject = $"New Finance Record for {month}/{year}";
                    body = $"Dear Client, a new financial record for {month}/{year} has been created.";
                }
                else if (eventType == "FinancialRecordUpdated")
                {
                    subject = $"Finance Record Update for {month}/{year}";
                    body = $"Dear Client, your financial record for {month}/{year} has been updated.";
                }
                else
                {
                    log.LogWarning($"Unknown event type: {eventType}");
                }
            }
            else
            {
                log.LogWarning("No 'Event' property found in message.");
            }

            var mail = new SendGridMessage();
            mail.SetFrom(_fromEmail);
            mail.AddTo(clientEmail);
            mail.SetSubject(subject);
            mail.AddContent("text/plain", body);

            var response = await _sendGridClient.SendEmailAsync(mail);
            var responseBody = await response.Body.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                log.LogError("SendGrid failed. StatusCode: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
            }
            log.LogInformation("Email sent to {Email} with status {Status}", clientEmail, response.StatusCode);
        }
    }
}
