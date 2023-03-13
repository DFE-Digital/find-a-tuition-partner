using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class SENDRequirementsModelValidator : AbstractValidator<SENDRequirementsModel>
{
    public SENDRequirementsModelValidator()
    {
        RuleFor(request => request.SENDRequirements)
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"Do you need tuition partners who can support pupils with SEND must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}