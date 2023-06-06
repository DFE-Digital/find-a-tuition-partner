using Application.Common.Models.Enquiry.Build;
using Application.Extensions;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class SchoolPostcodeModelValidator : AbstractValidator<SchoolPostcodeModel>
{
    public SchoolPostcodeModelValidator()
    {
        RuleFor(m => m.SchoolPostcode)
            .NotEmpty()
            .WithMessage("Enter a postcode");

        RuleFor(m => m.SchoolPostcode)
            .Must(m => !string.IsNullOrEmpty(m.ToSanitisedPostcode()))
            .WithMessage("Enter a real postcode");
    }
}