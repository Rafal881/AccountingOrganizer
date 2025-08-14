using ClientOrganizer.API.Models.Dtos;
using FluentValidation;

namespace ClientOrganizer.API.Application.Validators
{
    public class ClientCreateDtoValidator : AbstractValidator<ClientCreateDto>
    {
        public ClientCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.NipNb)
                .NotEmpty()
                .Matches("^[0-9]{10}$").WithMessage("Nip must be 10 digits");
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
