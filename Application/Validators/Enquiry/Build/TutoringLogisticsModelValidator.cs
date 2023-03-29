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
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r", " ").Length <= IntegerConstants.EnquiryQuestionsMaxCharacterSize)
            .WithMessage($"The type of tuition plan must be {IntegerConstants.EnquiryQuestionsMaxCharacterSize:N0} characters or less");
    }
}