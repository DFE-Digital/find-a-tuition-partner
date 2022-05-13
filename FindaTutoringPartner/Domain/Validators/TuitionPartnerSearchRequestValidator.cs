using Domain.Search;
using FluentValidation;

namespace Domain.Validators;

public class TuitionPartnerSearchRequestValidator<T> : AbstractValidator<T> where T : TuitionPartnerSearchRequest
{
    public TuitionPartnerSearchRequestValidator()
    {
        RuleFor(_ => _.Page).GreaterThan(0);
    }
}