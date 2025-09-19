namespace ClientOrganizer.FunctionApp.Models
{
    public class FinanceQueueMessage
    {
        public required string Email { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public FinanceEventType EventType { get; set; }
    }
}
