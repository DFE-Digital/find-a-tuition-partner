using Domain.Search;
using FluentValidation;

namespace Domain.Validators;

public class TuitionPartnerSearchRequestValidator<T> : AbstractValidator<T> where T : TuitionPartnerSearchRequest
{
    public TuitionPartnerSearchRequestValidator()
    {
        
    }
}