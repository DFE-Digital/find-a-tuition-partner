using Domain.Constants;
using FluentValidation;

namespace Domain.Validators;

public class GeneralInformationAboutSchoolValidator : AbstractValidator<School>
{
    public GeneralInformationAboutSchoolValidator()
    {
        RuleFor(m => m.EstablishmentName)
            .NotEmpty()
            .WithMessage("Enter a name");

        RuleFor(m => m.Urn)
            .NotEmpty()
            .WithMessage("Enter a URN");

        RuleFor(m => m.Address)
           .NotEmpty()
           .WithMessage("Enter a valid Address");

        RuleFor(m => m.EstablishmentTypeGroupId)
           .Must(ValidEstablishmentTypeGroupId)
           .WithMessage("Enter a valid EstablishmentTypeGroup");

        RuleFor(m => m.EstablishmentStatusId)
           .Must(ValidEstablishmentStatusId)
           .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.PhaseOfEducationId)
          .Must(ValidPhaseOfEducationId)
          .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.LocalAuthorityId)
          .NotEmpty()
          .WithMessage("Enter a valid LocalEducationAuthority");

        RuleFor(m => m.LocalAuthorityDistrictId)
         .NotEmpty()
         .WithMessage("Enter a valid LocalAuthorityDistrict");

        RuleFor(m => m.Postcode)
        .NotEmpty()
        .WithMessage("Enter a valid Postcode");
    }

    private bool ValidEstablishmentStatusId(int EstablishmentStatusId)
    {
        return Enum.GetName(typeof(EstablishmentsStatus), EstablishmentStatusId) != null;    
    }

    private bool ValidEstablishmentTypeGroupId(int EstablishmentTypeGroupId)
    {
        return Enum.GetName(typeof(EstablishmentTypeGroups), EstablishmentTypeGroupId) != null;
    }
    private bool ValidPhaseOfEducationId(int PhaseOfEducationId)
    {
        return Enum.GetName(typeof(PhasesOfEducation), PhaseOfEducationId) != null;
    }
}
