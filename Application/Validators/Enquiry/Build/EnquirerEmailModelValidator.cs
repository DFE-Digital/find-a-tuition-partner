using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class EnquirerEmailModelValidator : AbstractValidator<EnquirerEmailModel>
{
    public EnquirerEmailModelValidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email address is required")
             .Matches(StringConstants.EmailRegExp).WithMessage("You must enter an email address in the correct format.  Emails are usually in a format, like, username@example.com");
    }
}