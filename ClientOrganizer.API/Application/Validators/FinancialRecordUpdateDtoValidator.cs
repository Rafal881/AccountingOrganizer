using ClientOrganizer.API.Models.Dtos;
using FluentValidation;

namespace ClientOrganizer.API.Application.Validators
{
    public class FinancialRecordUpdateDtoValidator : AbstractValidator<FinancialRecordUpdateDto>
    {
        public FinancialRecordUpdateDtoValidator()
        {
            RuleFor(x => x.Month).InclusiveBetween(1, 12);
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
            RuleFor(x => x.IncomeTax).GreaterThanOrEqualTo(0).When(x => x.IncomeTax.HasValue);
            RuleFor(x => x.Vat).GreaterThanOrEqualTo(0).When(x => x.Vat.HasValue);
            RuleFor(x => x.InsuranceAmount).GreaterThanOrEqualTo(0).When(x => x.InsuranceAmount.HasValue);
            RuleFor(x => x)
                .Must(x => x.IncomeTax.HasValue || x.Vat.HasValue || x.InsuranceAmount.HasValue)
                .WithMessage("At least one property must be provided for update.");
        }
    }
}
