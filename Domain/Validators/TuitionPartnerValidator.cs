using FluentValidation;
using System.Text.RegularExpressions;

namespace Domain.Validators;

public class TuitionPartnerValidator : AbstractValidator<TuitionPartner>
{
    public TuitionPartnerValidator()
    {
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
    }

    private bool BeValidWebsite(string website)
    {
        if (string.IsNullOrEmpty(website))
        {
            return false;
        }

        return CheckRegex(website,
                             @"^(www|http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.org]+(\[\?%&=]*)?");
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
}
