using Application.Common.Models;
using Application.Extensions;
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
            .Must(m => !string.IsNullOrEmpty(m.ToSanitisedPostcode()))
            .WithMessage("Enter a real postcode");

        RuleForEach(m => m.Subjects)
            .Must(x => KeyStageSubject.TryParse(x, out var _));
    }
}