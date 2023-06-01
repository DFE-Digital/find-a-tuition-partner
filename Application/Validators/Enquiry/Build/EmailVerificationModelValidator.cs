using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class EmailVerificationModelValidator : AbstractValidator<EmailVerificationModel>
{
    public EmailVerificationModelValidator()
    {
        RuleFor(request => request.Passcode)
            .NotEmpty().WithMessage("Enter your passcode")
            .Matches(StringConstants.NumberRegExp).WithMessage("The passcode must be a number");
    }
}