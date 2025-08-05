namespace ClientOrganizer.API.Models.Dtos
{
    public class FinancialRecordDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal Vat { get; set; }
        public decimal InsuranceAmount { get; set; }
    }
}
