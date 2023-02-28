using Application.Common.Models.Enquiry.Build;
using FluentValidation;

namespace Application.Validators;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("A valid email is required");

        RuleFor(request => request.EnquiryText)
            .NotEmpty()
            .WithMessage("Enquiry Text is required.");

        RuleFor(m => m.KeyStages)
            .NotEmpty()
            .WithMessage("Select at least one key stage");

        RuleFor(m => m.Subjects)
            .NotEmpty()
            .WithMessage("Select the subject or subjects");
    }
}