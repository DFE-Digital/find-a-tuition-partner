using Application.Common.Models.Enquiry.Build;
using FluentValidation;

namespace Application.Validators.Enquiry.Build;

public class ConfirmSchoolModelValidator : AbstractValidator<ConfirmSchoolModel>
{
    public ConfirmSchoolModelValidator()
    {
        RuleFor(request => request.ConfirmedIsSchool)
            .NotNull()
            .When(m => m.HasSingleSchool)
            .WithMessage("Select to confirm if this is your school or not");

        RuleFor(request => request.SchoolId)
            .NotNull()
            .When(m => !m.HasSingleSchool)
            .WithMessage("Select to confirm which school you work for");
    }
}