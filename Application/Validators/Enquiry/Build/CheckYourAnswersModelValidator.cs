using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        RuleFor(m => m.HasKeyStageSubjects)
            .NotEqual(false)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Select at least one key stage and related subject");

        RuleFor(m => m.TuitionSetting)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Select a tuition setting option");

        RuleFor(request => request.Email)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter an email address")
            .Matches(StringConstants.EmailRegExp)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("You must enter an email address in the correct format");

        RuleFor(request => request.TutoringLogistics)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter the type of tuition plan that you need")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"The type of tuition plan must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.SENDRequirements)
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryQuestionsMaxCharacterSize))
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"SEND requirements must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.AdditionalInformation)
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryQuestionsMaxCharacterSize))
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"Any other considerations for tuition partners to consider must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(m => m.ConfirmTermsAndConditions)
            .NotEqual(false)
            .WithMessage("Select to confirm that you have not included any information that would allow anyone to identify pupils, such as names or specific characteristics");
    }
}