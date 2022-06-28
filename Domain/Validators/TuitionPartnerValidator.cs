using FluentValidation;

namespace Domain.Validators;

public class TuitionPartnerValidator : AbstractValidator<TuitionPartner>
{
    public TuitionPartnerValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .WithMessage("Enter a name");

        RuleFor(m => m.Description)
            .NotEmpty()
            .WithMessage("Enter a description");
    }
}