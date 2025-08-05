namespace ClientOrganizer.API.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NipNb { get; set; }
        public string Email { get; set; }

        public ICollection<FinancialData> FinancialRecords { get; set; }
    }
}
