using Application.Common.Models.Enquiry.Build;
using FluentValidation;

namespace Application.Validators;

public class EnquiryQuestionModelValidator : AbstractValidator<EnquiryQuestionModel>
{
    public EnquiryQuestionModelValidator()
    {
        RuleFor(request => request.EnquiryText)
             .NotEmpty()
             .WithMessage("Enquiry Text is required.");
    }
}