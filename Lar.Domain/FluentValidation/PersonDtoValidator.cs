using FluentValidation;
using Lar.Domain.Dto;

namespace Lar.WebApi.Validation
{
    public class PersonDtoValidator : AbstractValidator<PersonDto>
    {
        public PersonDtoValidator()
        {
            RuleFor(person => person.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(person => person.CPF)
                .NotEmpty().WithMessage("CPF is required.")
                .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("Invalid CPF format. Please use the format XXX.XXX.XXX-XX.");
        }
    }
}
