using Domain.Search;
using FluentValidation;

namespace Domain.Validators;

public abstract class SearchRequestBaseValidator<T> : AbstractValidator<T> where T : SearchRequestBase
{
    protected SearchRequestBaseValidator()
    {
        RuleFor(_ => _.Page).GreaterThan(-1);
        RuleFor(_ => _.PageSize).InclusiveBetween(1, SearchRequestBase.MaxPageSize);
    }
}