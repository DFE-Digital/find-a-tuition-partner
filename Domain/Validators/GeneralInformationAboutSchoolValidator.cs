using System.Text.RegularExpressions;
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
           .NotEmpty()
           .WithMessage("Enter a valid EstablishmentTypeGroup");

        RuleFor(m => m.EstablishmentStatusId)
           .NotEmpty()
           .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.PhaseOfEducationId)
          .NotEmpty()
          .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.LocalAuthorityId)
          .NotEmpty()
          .WithMessage("Enter a valid LocalEducationAuthority");

        //RuleFor(m => m.LocalAuthorityDistrict)
        // .NotEmpty()
        // .WithMessage("Enter a valid LocalAuthorityDistrict");
    }
}
