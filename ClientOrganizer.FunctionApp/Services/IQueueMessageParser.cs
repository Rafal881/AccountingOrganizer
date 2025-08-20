using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public interface IQueueMessageParser
    {
        FinanceQueueMessage? Parse(string queueMessage, ILogger log);
    }
}
