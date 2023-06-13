using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Respond;

public class CheckYourAnswersModelValidator : AbstractValidator<CheckYourAnswersModel>
{
    public CheckYourAnswersModelValidator()
    {
        RuleFor(request => request.KeyStageAndSubjectsText)
            .NotEmpty()
            .WithMessage("Enter can you support those key stages and subjects")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize)
            .WithMessage($"Key stages and subjects must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TuitionSettingText)
            .NotEmpty()
            .WithMessage("Enter to what extent can you support that tuition setting")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize)
            .WithMessage($"Tuition setting must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsText)
            .NotEmpty()
            .WithMessage("Enter can you support that tuition plan")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize)
            .WithMessage($"Tuition plan must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.SENDRequirementsText)
            .NotEmpty()
            .When(m => !string.IsNullOrWhiteSpace(m.EnquirySENDRequirements))
            .WithMessage("Enter can you support the SEND requirements")
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize))
            .WithMessage($"SEND requirements must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.AdditionalInformationText)
            .NotEmpty()
            .When(m => !string.IsNullOrWhiteSpace(m.EnquiryAdditionalInformation))
            .WithMessage("Enter can you support the other school considerations")
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize))
            .WithMessage($"Other school considerations must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");
    }
}