using Application.Common.Models.Enquiry.Manage;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Manage;

public class NotInterestedFeedbackModelValidator : AbstractValidator<NotInterestedFeedbackModel>
{
    public NotInterestedFeedbackModelValidator()
    {
        RuleFor(request => request.EnquirerNotInterestedReasonAdditionalInfo)
            .NotEmpty()
            .When(m => m.MustCollectAdditionalInfo)
            .WithMessage("Enter your reasons for removing the tuition partner to submit your feedback ")
            .Must(x => !string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.SmallTextAreaMaxCharacterSize)
            .When(m => m.MustCollectAdditionalInfo)
            .WithMessage($"Your reasons for removing the tuition partner must be {IntegerConstants.SmallTextAreaMaxCharacterSize:N0} characters or less");
    }
}