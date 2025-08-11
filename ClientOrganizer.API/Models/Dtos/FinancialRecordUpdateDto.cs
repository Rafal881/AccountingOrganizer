using System.ComponentModel.DataAnnotations;

namespace ClientOrganizer.API.Models.Dtos
{
    public class FinancialRecordUpdateDto
    {
        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? IncomeTax { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Vat { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? InsuranceAmount { get; set; }
    }
}
