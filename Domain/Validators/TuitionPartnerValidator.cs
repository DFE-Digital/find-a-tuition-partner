using System.Text.RegularExpressions;
using Domain.Constants;
using FluentValidation;

namespace Domain.Validators;

public class TuitionPartnerValidator : AbstractValidator<TuitionPartner>
{
    private readonly Dictionary<string, TuitionPartner> _successfullyProcessed;

    public TuitionPartnerValidator(Dictionary<string, TuitionPartner> successfullyProcessed)
    {
        _successfullyProcessed = successfullyProcessed;

        RuleFor(m => m.Name)
            .NotEmpty()
            .WithMessage("Enter a name");

        RuleFor(m => m.Description)
            .NotEmpty()
            .WithMessage("Enter a description");

        RuleFor(m => m.Website)
           .Must(BeValidWebsite)
           .WithMessage("Enter a valid website");

        RuleFor(m => m.Email)
            .EmailAddress().When(m => !string.IsNullOrEmpty(m.Email) && m.Email != " ")
            .WithMessage("Enter a valid email or remove");

        RuleFor(m => m.Prices)
            .Must(HasValidPrice)
            .WithMessage("Enter a price greater then zero");

        RuleFor(m => m.LocalAuthorityDistrictCoverage)
            .Must(m => m.Any())
            .WithMessage("Enter Local Authority District Coverage");

        RuleFor(m => m.SubjectCoverage)
            .Must(m => m.Any())
            .WithMessage("Enter subject Coverage");

        RuleFor(m => m.OrganisationTypeId)
            .Must(ValidOrganisationTypeId)
            .WithMessage("Enter a valid Organisation Type");

        RuleFor(m => m.ImportId)
            .Must(NotBeADuplicateImportId)
            .WithMessage(DuplicateImportIdErrorMessage);

        RuleFor(m => m.SeoUrl)
            .Must(NotBeADuplicateSeoUrl)
            .WithMessage(DuplicateSeoUrlErrorMessage);
    }

    private bool BeValidWebsite(string website)
    {
        if (string.IsNullOrEmpty(website))
        {
            return false;
        }

        return CheckRegex(website,
                             @"^https?://([\w-]+\.)+[\w-]+[.com|.org]+(\[\?%&=]*)?");
    }
    private bool HasValidPrice(ICollection<Price> prices)
    {
        if (prices.Count == 0)
        {
            return false;
        }
        return prices.Any(x => x.GroupSize > 0 && x.HourlyRate > 0);
    }

    private static bool CheckRegex(string property, string regex)
    {
        return Regex.IsMatch(property,
                         regex,
                         RegexOptions.IgnoreCase);
    }

    private bool ValidOrganisationTypeId(int organisationTypeId)
    {
        return Enum.GetName(typeof(OrganisationTypes), organisationTypeId) != null;
    }

    private bool NotBeADuplicateImportId(string importId)
    {
        return !_successfullyProcessed.Any(x => string.Equals(x.Value.ImportId, importId, StringComparison.InvariantCultureIgnoreCase));
    }

    private bool NotBeADuplicateSeoUrl(string seoUrl)
    {
        return !_successfullyProcessed.Any(x => string.Equals(x.Value.SeoUrl, seoUrl, StringComparison.InvariantCultureIgnoreCase));
    }

    private string DuplicateImportIdErrorMessage(TuitionPartner tuitionPartner)
    {
        return $"Duplicate import id in {_successfullyProcessed.First(x => string.Equals(x.Value.ImportId, tuitionPartner.ImportId, StringComparison.InvariantCultureIgnoreCase)).Key}";
    }

    private string DuplicateSeoUrlErrorMessage(TuitionPartner tuitionPartner)
    {
        return $"Duplicate seo url in {_successfullyProcessed.First(x => string.Equals(x.Value.SeoUrl, tuitionPartner.SeoUrl, StringComparison.InvariantCultureIgnoreCase)).Key}";
    }
}
