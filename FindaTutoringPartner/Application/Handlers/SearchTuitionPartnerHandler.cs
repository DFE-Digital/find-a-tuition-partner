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
                foreach (var subjectId in request.SubjectIds)
                {
                    switch (subjectId)
                    {
                        case Subjects.Id.PrimaryLiteracy: coverageQueryable = coverageQueryable.Where(e => e.PrimaryLiteracy == true); break;
                        case Subjects.Id.PrimaryNumeracy: coverageQueryable = coverageQueryable.Where(e => e.PrimaryNumeracy == true); break;
                        case Subjects.Id.PrimaryScience: coverageQueryable = coverageQueryable.Where(e => e.PrimaryScience == true); break;
                        case Subjects.Id.SecondaryEnglish: coverageQueryable = coverageQueryable.Where(e => e.SecondaryEnglish == true); break;
                        case Subjects.Id.SecondaryHumanities: coverageQueryable = coverageQueryable.Where(e => e.SecondaryHumanities == true); break;
                        case Subjects.Id.SecondaryMaths: coverageQueryable = coverageQueryable.Where(e => e.SecondaryMaths == true); break;
                        case Subjects.Id.SecondaryModernForeignLanguages: coverageQueryable = coverageQueryable.Where(e => e.SecondaryModernForeignLanguages == true); break;
                        case Subjects.Id.SecondaryScience: coverageQueryable = coverageQueryable.Where(e => e.SecondaryScience == true); break;
                    }
                }

                returnAll = false;
            }

            if (request.TuitionTypeIds != null)
            {
                returnAll = false;
            }

            IQueryable<TuitionPartner> queryable;

            if (returnAll)
            {
                queryable = _dbContext.TuitionPartners.AsQueryable();
            }
            else
            {
                var groupQueryable = coverageQueryable.GroupBy(e => e.TuitionPartnerId);

                if (request.TuitionTypeIds != null)
                {
                    foreach (var tuitionTypeId in request.TuitionTypeIds)
                    {
                        switch (tuitionTypeId)
                        {
                            case TuitionTypes.Id.Online:
                                groupQueryable = groupQueryable.Where(g => g.Any(e => e.TuitionTypeId == TuitionTypes.Id.Online));
                                break;
                            case TuitionTypes.Id.InPerson:
                                groupQueryable = groupQueryable.Where(g => g.Any(e => e.TuitionTypeId == TuitionTypes.Id.InPerson));
                                break;
                        }
                    }
                }

                var tuitionPartnerIds = await groupQueryable.Select(g => g.Key).ToArrayAsync(cancellationToken);

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