using Azure.Messaging.ServiceBus;
using ClientOrganizer.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientOrganizer.API.Application.Services.Outbox
{
    public class OutboxProcessor : BackgroundService
    {
        private const int MaxRetryCount = 5;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ClientOrganizerDbContext>();
                    var sender = scope.ServiceProvider.GetRequiredService<ServiceBusSender>();

                    var pendingMessages = await dbContext.OutboxMessages
                        .Where(message => message.ProcessedOnUtc == null && message.RetryCount < MaxRetryCount)
                        .OrderBy(message => message.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    if (pendingMessages.Count == 0)
                    {
                        await Task.Delay(PollInterval, stoppingToken);
                        continue;
                    }

                    foreach (var message in pendingMessages)
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }

                        try
                        {
                            var serviceBusMessage = new ServiceBusMessage(message.Payload)
                            {
                                ContentType = "application/json",
                                MessageId = message.Id.ToString(),
                                Subject = message.EventType
                            };

                            serviceBusMessage.ApplicationProperties["EventType"] = message.EventType;

                            await sender.SendMessageAsync(serviceBusMessage, stoppingToken);

                            message.ProcessedOnUtc = DateTime.UtcNow;
                            message.Error = null;
                        }
                        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                        {
                            message.RetryCount++;
                            message.Error = ex.Message;

                            if (message.RetryCount >= MaxRetryCount)
                            {
                                _logger.LogError(ex, "Failed to dispatch outbox message {MessageId}. Max retry count reached.", message.Id);
                            }
                            else
                            {
                                _logger.LogWarning(ex, "Failed to dispatch outbox message {MessageId}. Will retry.", message.Id);
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while processing the outbox.");
                    await Task.Delay(PollInterval, stoppingToken);
                }
            }
        }
    }
}
