using ClientOrganizer.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid;
using ClientOrganizer.FunctionApp.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace ClientOrganizer.FunctionApp
{
    public class FinanceEmailFunction
    {
        private readonly SendGridClient _sendGridClient;
        private readonly IEmailContentProvider _contentProvider;
        private readonly IQueueMessageParser _messageParser;
        private readonly ISendGridMessageFactory _messageFactory;
        private readonly string _fromEmail;

        public FinanceEmailFunction(
            SendGridClient sendGridClient,
            IEmailContentProvider contentProvider,
            IQueueMessageParser messageParser,
            ISendGridMessageFactory messageFactory,
            IOptions<SendGridOptions> sendGridOptions)
        {
            _sendGridClient = sendGridClient;
            _contentProvider = contentProvider;
            _messageParser = messageParser;
            _messageFactory = messageFactory;
            _fromEmail = sendGridOptions.Value.FromEmail;
            if (string.IsNullOrWhiteSpace(_fromEmail))
            {
                throw new InvalidOperationException("SendGrid FromEmail is not configured.");
            }
        }

        [Function("FinanceEmailFunction")]
        public async Task Run(
            [ServiceBusTrigger("%FinanceQueueName%", Connection = "serviceBusConnectionString")] string queueMessage,
            FunctionContext context)
        {
            var log = context.GetLogger("FinanceEmailFunction");

            var parsedMessage = _messageParser.Parse(queueMessage, log);
            if (parsedMessage is null)
                return;

            var emailContent = _contentProvider.GetContent(parsedMessage.EventType, parsedMessage.Month, parsedMessage.Year, log);

            if (emailContent == null || string.IsNullOrEmpty(emailContent.Subject) || string.IsNullOrEmpty(emailContent.Body))
                return;

            var mail = _messageFactory.Create(_fromEmail, parsedMessage.Email, emailContent);

            var response = await _sendGridClient.SendEmailAsync(mail);
            var responseBody = await response.Body.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                log.LogError("SendGrid failed. StatusCode: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
            }
            log.LogInformation("Email sent to {Email} with status {Status}", parsedMessage.Email, response.StatusCode);
        }
    }
}
