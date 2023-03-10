using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        //TODO - Postcode needs to be validated, but not shown on check your answers page, so what to do if invalid?
        RuleFor(m => m.Postcode)
            .NotEmpty()
            .WithMessage("A postcode is required");

        //TODO - Key stages and subjects can be cleared on the search pages - how does that flow work?
        RuleFor(m => m.KeyStages)
            .NotEmpty()
            .WithMessage("Select at least one key stage");

        RuleFor(m => m.Subjects)
            .NotEmpty()
            .WithMessage("Select the subject or subjects");

        RuleFor(m => m.TuitionType)
            .NotEmpty()
            .WithMessage("A type of tuition is required");

        RuleFor(request => request.Email).NotEmpty().WithMessage("Email address is required")
            .Matches(StringConstants.EmailRegExp).WithMessage("You must enter an email address in the correct format");

        RuleFor(request => request.TutoringLogistics)
             .NotEmpty()
             .WithMessage("What type of tuition plan do you need is required")
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"What type of tuition plan do you need must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.SendRequirements)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Do you need tuition partners who can support with SEND must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.AdditionalInformation)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Is there anything else that you want tuition partners to consider must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}