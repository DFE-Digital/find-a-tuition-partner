using Application.Common.Models.Enquiry.Build;
using Application.Constants;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class AdditionalInformationModelValidator : AbstractValidator<AdditionalInformationModel>
{
    public AdditionalInformationModelValidator()
    {
        RuleFor(request => request.AdditionalInformation)
            .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && x.Replace("\r\n", "\n").Length <= IntegerConstants.LargeTextAreaMaxCharacterSize))
            .WithMessage($"Other tuition requirements must be {IntegerConstants.LargeTextAreaMaxCharacterSize:N0} characters or less");
    }
}