using Domain.Search;
using FluentValidation;

namespace Domain.Validators;

public abstract class TuitionPartnerSearchRequestValidator<T> : AbstractValidator<T> where T : TuitionPartnerSearchRequest
{
    protected TuitionPartnerSearchRequestValidator()
    {
        RuleFor(_ => _.Page).GreaterThan(0);
    }
}