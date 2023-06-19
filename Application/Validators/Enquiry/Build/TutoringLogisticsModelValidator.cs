using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class TutoringLogisticsModelValidator : AbstractValidator<TutoringLogisticsModel>
{
    public TutoringLogisticsModelValidator()
    {
        RuleFor(request => request.TutoringLogisticsDetailsModel.NumberOfPupils)
            .NotEmpty()
            .WithMessage("Enter the number of pupils that need tuition")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.SmallTextAreaMaxCharacterSize)
            .WithMessage($"The number of pupils that need tuition must be {IntegerConstants.SmallTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.StartDate)
            .NotEmpty()
            .WithMessage("Enter when you want tuition to start")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.SmallTextAreaMaxCharacterSize)
            .WithMessage($"When you want tuition to start must be {IntegerConstants.SmallTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.TuitionDuration)
            .NotEmpty()
            .WithMessage("Enter how long you need tuition for")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.SmallTextAreaMaxCharacterSize)
            .WithMessage($"How long you need tuition for must be {IntegerConstants.SmallTextAreaMaxCharacterSize:N0} characters or less");

        RuleFor(request => request.TutoringLogisticsDetailsModel.TimeOfDay)
            .NotEmpty()
            .WithMessage("Enter what time of day you need tuition")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.SmallTextAreaMaxCharacterSize)
            .WithMessage($"What time of day you need tuition must be {IntegerConstants.SmallTextAreaMaxCharacterSize:N0} characters or less");
    }
}