using ClientOrganizer.FunctionApp.Models;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.FunctionApp.Services
{
    public interface IEmailContentProvider
    {
        EmailContent? GetContent(FinanceEventType eventType, int month, int year, ILogger log);
    }
}
