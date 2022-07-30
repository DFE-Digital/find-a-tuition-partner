using Application.Repositories;
using Domain;
using Domain.Constants;
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

    public class Command : TuitionPartnerSearchRequest, IRequest<TuitionPartnerSearchResultsPage>
    {

    }

    public class Handler : IRequestHandler<Command, TuitionPartnerSearchResultsPage>
    {
        private readonly INtpDbContext _dbContext;
        private readonly ITuitionPartnerRepository _repository;

        public Handler(INtpDbContext dbContext, ITuitionPartnerRepository repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public async Task<TuitionPartnerSearchResultsPage> Handle(Command request, CancellationToken cancellationToken)
        {
            var queryable = _dbContext.TuitionPartners.AsQueryable();

            LocalAuthorityDistrict? lad = null;

            if (request.LocalAuthorityDistrictCode != null)
            {
                lad = await _dbContext.LocalAuthorityDistricts
                    .Include(e => e.LocalAuthority)
                    .SingleOrDefaultAsync(e => e.Code == request.LocalAuthorityDistrictCode, cancellationToken);

                if (lad != null)
                {
                    queryable = queryable.Where(e => e.LocalAuthorityDistrictCoverage.Any(x => x.LocalAuthorityDistrictId == lad.Id && (request.TuitionTypeId == null || x.TuitionTypeId == request.TuitionTypeId)));
                }
            }

            if (request.SubjectIds != null)
            {
                foreach (var subjectId in request.SubjectIds)
                {
                    // Must support all selected subjects for the tuition type if selected
                    // TODO: This is a slow query that gets worse as multiple subjects are selected. Will need optimising either via pulling the data back and querying in memory or denormalising the data
                    queryable = queryable.Where(e => e.SubjectCoverage.Any(x => x.SubjectId == subjectId && (request.TuitionTypeId == null || x.TuitionTypeId == request.TuitionTypeId)));
                }
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
            var ids = await queryable.Skip(request.Page * request.PageSize).Take(request.PageSize).Select(e => e.Id).ToArrayAsync(cancellationToken);
            var results = (await _repository.GetSearchResultsDictionaryAsync(ids, lad?.Id, request.OrderBy, request.Direction, cancellationToken)).Values.ToArray();

            return new TuitionPartnerSearchResultsPage(request, count, results, lad);
        }
    }
}