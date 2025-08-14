using ClientOrganizer.API.Models.Dtos;
using FluentValidation;

namespace ClientOrganizer.API.Application.Validators
{
    public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
    {
        public ClientUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
            RuleFor(x => x.Address).NotEmpty().When(x => x.Address is not null);
            RuleFor(x => x.NipNb)
                .Matches("^[0-9]{10}$").When(x => x.NipNb is not null)
                .WithMessage("Nip must be 10 digits");
            RuleFor(x => x.Email)
                .EmailAddress().When(x => x.Email is not null);
            RuleFor(x => x)
                .Must(x => x.Name is not null || x.Address is not null || x.NipNb is not null || x.Email is not null)
                .WithMessage("At least one property must be provided for update.");
        }
    }
}
