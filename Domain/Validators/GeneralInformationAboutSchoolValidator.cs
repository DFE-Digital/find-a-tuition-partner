using System.Text.RegularExpressions;
using FluentValidation;

namespace Domain.Validators;

public class GeneralInformationAboutSchoolValidator : AbstractValidator<GeneralInformationAboutSchools>
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

        RuleFor(m => m.EstablishmentTypeGroup)
           .NotEmpty()
           .WithMessage("Enter a valid EstablishmentTypeGroup");

        RuleFor(m => m.EstablishmentStatus)
           .NotEmpty()
           .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.PhaseOfEducation)
          .NotEmpty()
          .WithMessage("Enter a valid EstablishmentStatus");

        RuleFor(m => m.LocalEducationAuthority)
          .NotEmpty()
          .WithMessage("Enter a valid LocalEducationAuthority");

        RuleFor(m => m.LocalAuthorityDistrict)
         .NotEmpty()
         .WithMessage("Enter a valid LocalAuthorityDistrict");
    }
}
