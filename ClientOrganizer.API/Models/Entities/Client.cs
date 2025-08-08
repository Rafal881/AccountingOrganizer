namespace ClientOrganizer.API.Models.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string NipNb { get; set; }
        public required string Email { get; set; }

        public ICollection<FinancialData>? FinancialRecords { get; set; }
    }
}
