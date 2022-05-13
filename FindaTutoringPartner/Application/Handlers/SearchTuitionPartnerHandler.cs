using Domain;
using Domain.Search;
using Domain.Validators;
using MediatR;

namespace Application.Handlers;

public class SearchTuitionPartnerHandler
{
    public class CommandValidator : TuitionPartnerSearchRequestValidator<Command>
    {

    }

    public class Command : TuitionPartnerSearchRequest, IRequest<SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>>
    {

    }

    public async Task<SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>> Handle(Command request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}