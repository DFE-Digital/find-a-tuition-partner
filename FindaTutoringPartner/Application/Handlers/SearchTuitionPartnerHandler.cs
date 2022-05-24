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
            var coverageQueryable = _dbContext.TuitionPartnerCoverage.AsQueryable();

            var returnAll = true;

            if (request.LocalAuthorityDistrictCode != null)
            {
                var lad = await _dbContext.LocalAuthorityDistricts.SingleOrDefaultAsync(e => e.Code == request.LocalAuthorityDistrictCode, cancellationToken);
                if (lad != null)
                {
                    coverageQueryable = coverageQueryable.Where(e => e.LocalAuthorityDistrictId == lad.Id);
                    returnAll = false;
                }
            }

            if (request.SubjectIds != null)
            {
                coverageQueryable = coverageQueryable.Where(e => request.SubjectIds.Contains(e.SubjectId));
                returnAll = false;
            }

            if (request.TuitionTypeIds != null)
            {
                coverageQueryable = coverageQueryable.Where(e => request.TuitionTypeIds.Contains(e.TuitionTypeId));
                returnAll = false;
            }

            IQueryable<TuitionPartner> queryable;

            if (returnAll)
            {
                queryable = _dbContext.TuitionPartners.AsQueryable();
            }
            else
            {
                var tuitionPartnerIds = await coverageQueryable.Select(e => e.TuitionPartnerId).Distinct().ToArrayAsync(cancellationToken);
                queryable = _dbContext.TuitionPartners.Where(e => tuitionPartnerIds.Contains(e.Id));
            }

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