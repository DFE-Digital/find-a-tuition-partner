using Domain.Search;

namespace Domain.Validators;

public abstract class TuitionPartnerSearchRequestValidator<T> : SearchRequestBaseValidator<T> where T : TuitionPartnerSearchRequest
{
    protected TuitionPartnerSearchRequestValidator()
    {
    }
}