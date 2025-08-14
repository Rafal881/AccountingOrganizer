using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public interface IQueueMessageParser
    {
        bool TryParse(string queueMessage, ILogger log, out string clientEmail, out int month, out int year, out FinanceEventType eventType);
    }
}
