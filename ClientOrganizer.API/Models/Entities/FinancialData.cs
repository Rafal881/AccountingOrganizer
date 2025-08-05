namespace ClientOrganizer.API.Models.Entities
{
    public class FinancialData
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal Vat { get; set; }
        public decimal InsuranceAmount { get; set; }

        public Client Client { get; set; }
    }
}
