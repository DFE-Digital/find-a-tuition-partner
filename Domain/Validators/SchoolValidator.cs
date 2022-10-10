using System.Text.RegularExpressions;
using Domain.Constants;
using FluentValidation;

namespace Domain.Validators;

public class SchoolValidator : AbstractValidator<School>
{
    public SchoolValidator()
    {
        RuleFor(m => m.EstablishmentName)
            .NotEmpty()
            .WithMessage("Enter a name");

        RuleFor(m => m.Urn)
            .GreaterThan(0)
            .WithMessage("Enter a URN");

        RuleFor(m => m.Address)
            .NotEmpty()
            .WithMessage("Enter a valid Address");

        RuleFor(m => m.EstablishmentTypeGroupId)
            .Must(ValidEstablishmentTypeGroupId)
            .WithMessage("Enter a valid Establishment Type Group Id");

        RuleFor(m => m.EstablishmentStatusId)
            .Must(ValidEstablishmentStatusId)
            .WithMessage("Enter a valid Establishment Status");

        RuleFor(m => m.PhaseOfEducationId)
            .Must(ValidPhaseOfEducationId)
            .WithMessage("Enter a valid Phase of Education id");

        RuleFor(m => m.LocalAuthorityId)
            .GreaterThan(0)
            .WithMessage("Enter a valid Local Education Authority id");

        RuleFor(m => m.LocalAuthorityDistrictId)
            .GreaterThan(0)
            .WithMessage("Enter a valid Local Authority District id");

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

