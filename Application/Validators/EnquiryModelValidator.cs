using Application.Common.Models;
using FluentValidation;

namespace Application.Validators;

public class EnquiryModelValidator : AbstractValidator<EnquiryModel>
{
    public EnquiryModelValidator()
    {
        RuleFor(request => request.EnquiryText)
            .NotEmpty()
            .WithMessage("Enquiry Text is required.");

        RuleFor(request => request.Email).NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("A valid email is required");
    }
}