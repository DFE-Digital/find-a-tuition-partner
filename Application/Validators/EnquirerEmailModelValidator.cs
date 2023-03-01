using Application.Common.Models.Enquiry.Build;
using FluentValidation;

namespace Application.Validators;

public class EnquirerEmailModelValidator : AbstractValidator<EnquirerEmailModel>
{
    public EnquirerEmailModelValidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email address is required")
             .EmailAddress().WithMessage("A valid email is required");
    }
}