using Application.Common.Models;
using Application.Constants;
using Application.Queries;
using FluentValidation;

namespace Application.Validators;

public sealed class SearchResultValidator : AbstractValidator<GetSearchResultsQuery>
{
    public SearchResultValidator()
    {
        RuleFor(m => m.Postcode)
            .NotEmpty()
            .WithMessage("Enter a postcode");

        RuleFor(m => m.Postcode)
            .Matches(StringConstants.PostcodeRegExp)
            .WithMessage("Enter a real postcode")
            .When(m => !string.IsNullOrEmpty(m.Postcode));

        RuleForEach(m => m.Subjects)
            .Must(x => KeyStageSubject.TryParse(x, out var _));
    }
}