namespace ClientOrganizer.API.Models.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccurredOnUtc { get; set; }
        public string EventType { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime? ProcessedOnUtc { get; set; }
        public int RetryCount { get; set; }
        public string? Error { get; set; }

        public static OutboxMessage Create(string eventType, string payload)
            => new()
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = payload,
                OccurredOnUtc = DateTime.UtcNow,
                RetryCount = 0
            };
    }
}
