using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Respond;

public class ViewAndCaptureEnquiryResponseModelValidator : AbstractValidator<ViewAndCaptureEnquiryResponseModel>
{
    public ViewAndCaptureEnquiryResponseModelValidator()
    {
        RuleFor(request => request.KeyStageAndSubjectsText)
            .NotEmpty()
            .WithMessage("Enter can you support those key stages and subjects")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Key stages and subjects must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.TuitionTypeText)
            .NotEmpty()
            .WithMessage("Enter to what extent can you support that type of tuition")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Tuition type must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.TutoringLogisticsText)
            .NotEmpty()
            .WithMessage("Enter can you support that tuition plan")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Tuition plan must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.SENDRequirementsText)
            .NotEmpty()
            .When(m => !string.IsNullOrWhiteSpace(m.EnquirySENDRequirements))
            .WithMessage("Enter can you support the SEND requirements")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"SEND requirements must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");

        RuleFor(request => request.AdditionalInformationText)
            .NotEmpty()
            .When(m => !string.IsNullOrWhiteSpace(m.EnquiryAdditionalInformation))
            .WithMessage("Enter can you support the other school considerations")
            .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"Other school considerations must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}