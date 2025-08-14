using ClientOrganizer.API.Models.Dtos;
using FluentValidation;

namespace ClientOrganizer.API.Application.Validators
{
    public class FinancialRecordCreateDtoValidator : AbstractValidator<FinancialRecordCreateDto>
    {
        public FinancialRecordCreateDtoValidator()
        {
            RuleFor(x => x.Month).InclusiveBetween(1, 12);
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
            RuleFor(x => x.IncomeTax).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Vat).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InsuranceAmount).GreaterThanOrEqualTo(0);
        }
    }
}
