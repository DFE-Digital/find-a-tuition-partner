using Domain;
using Domain.Search;
using Domain.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers;

public class SearchTuitionPartnerHandler
{
    public class CommandValidator : TuitionPartnerSearchRequestValidator<Command>
    {

    }

    public class Command : TuitionPartnerSearchRequest, IRequest<SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>>
    {

    }

    public class Handler : IRequestHandler<Command, SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>>
    {
        private readonly INtpDbContext _dbContext;

        public Handler(INtpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>> Handle(Command request, CancellationToken cancellationToken)
        {
            var queryable = _dbContext.TuitionPartners.AsQueryable();

            switch (request.OrderBy)
            {
                case TuitionPartnerOrderBy.Name:
                    queryable = request.Direction == OrderByDirection.Descending
                        ? queryable.OrderByDescending(e => e.Name)
                        : queryable.OrderBy(e => e.Name);
                    break;
            }

            var count = await queryable.CountAsync(cancellationToken);
            var results = await queryable.Skip(request.Page * request.PageSize).Take(request.PageSize).ToArrayAsync(cancellationToken);

            return new SearchResultsPage<TuitionPartnerSearchRequest, TuitionPartner>(request, count, results);
        }
    }
}