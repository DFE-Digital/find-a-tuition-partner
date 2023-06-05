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

        RuleFor(m => m.TuitionType)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Select a type of tuition option");

        RuleFor(request => request.Email)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter an email address")
            .Matches(StringConstants.EmailRegExp)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("You must enter an email address in the correct format");

        RuleFor(request => request.TutoringLogisticsDetailsModel.NumberOfPupils)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter the number of pupils that need tuition")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryShortQuestionsMaxCharacterSize)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"The number of pupils that need tuition must be {IntegerConstants.EnquiryShortQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.StartDate)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter when you want tuition to start")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryShortQuestionsMaxCharacterSize)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"When you want tuition to start must be {IntegerConstants.EnquiryShortQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.TuitionDuration)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter how long you need tuition for")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryShortQuestionsMaxCharacterSize)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"How long you need tuition for must be {IntegerConstants.EnquiryShortQuestionsMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.TimeOfDay)
            .NotEmpty()
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage("Enter what time of day you need tuition")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.EnquiryShortQuestionsMaxCharacterSize)
            .When(m => m.ConfirmTermsAndConditions)
            .WithMessage($"What time of day you need tuition must be {IntegerConstants.EnquiryShortQuestionsMaxCharacterSize:N0} characters or less");

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