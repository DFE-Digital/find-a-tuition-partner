using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class AdditionalInformationModelValidator : AbstractValidator<AdditionalInformationModel>
{
    public AdditionalInformationModelValidator()
    {
        RuleFor(request => request.AdditionalInformation)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Is there anything else that you want tuition partners to consider must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}