using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class TutoringLogisticsModelValidator : AbstractValidator<TutoringLogisticsModel>
{
    public TutoringLogisticsModelValidator()
    {
        RuleFor(request => request.TutoringLogistics)
             .NotEmpty()
             .WithMessage("Enter the type of tuition plan that you need")
             .MaximumLength(IntegerConstants.EnquiryQuestionsMaxCharacterSize)
             .WithMessage($"What type of tuition plan do you need must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize} characters or less");
    }
}