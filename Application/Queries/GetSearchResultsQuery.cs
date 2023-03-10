using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Extensions;
using Application.Validators;
using Domain;
using Domain.Enums;
using Domain.Search;
using FluentValidation.Results;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using TuitionType = Domain.Enums.TuitionType;

namespace Application.Queries;

public record GetSearchResultsQuery : SearchModel, IRequest<SearchResultsModel>
{
    public GetSearchResultsQuery()
    {
    }

    public GetSearchResultsQuery(SearchModel query) : base(query)
    {
    }
    public TuitionType? PreviousTuitionType { get; set; } = null;
};

public class GetSearchResultsQueryHandler : IRequestHandler<GetSearchResultsQuery, SearchResultsModel>
{
    private readonly ILocationFilterService _locationService;
    private readonly ITuitionPartnerService _tuitionPartnerService;
    private readonly ILookupDataService _lookupDataService;

    public GetSearchResultsQueryHandler(ILocationFilterService locationService, ITuitionPartnerService tuitionPartnerService,
        ILookupDataService lookupDataService)
    {
        _locationService = locationService;
        _tuitionPartnerService = tuitionPartnerService;
        _lookupDataService = lookupDataService;
    }

    public async Task<SearchResultsModel> Handle(GetSearchResultsQuery request, CancellationToken cancellationToken)
    {
        var queryResponse = new SearchResultsModel(request);

        var searchResults = await GetSearchResults(request, cancellationToken);

        return searchResults switch
        {
            SuccessResult => queryResponse with
            {
                Results = searchResults.Data,
            },
            Domain.ValidationResult error => queryResponse with
            {
                Validation = new FluentValidationResult(error.Failures)
            },
            ErrorResult error => queryResponse with
            {
                Validation = new FluentValidationResult(new[]
                {
                        new ValidationFailure("Postcode", error.ToString()),
                    }),
            },
            _ => queryResponse with { Validation = UnknownError() },
        };

        static FluentValidationResult UnknownError() =>
            new(new[] { new ValidationFailure("", "An unknown problem occurred") });
    }

    private async Task<IResult<TuitionPartnersResult>> GetSearchResults(GetSearchResultsQuery request,
        CancellationToken cancellationToken)
    {
        var location = await GetSearchLocation(request, cancellationToken);

        if (location is IErrorResult error)
        {
            return error.Cast<TuitionPartnersResult>();
        }

        var results = await FindTuitionPartners(
            location.Data,
            request,
            cancellationToken);

        var result = new TuitionPartnersResult(results, location.Data.LocalAuthorityDistrict);

        return Result.Success(result);
    }

    private async Task<IResult<LocationFilterParameters>> GetSearchLocation(GetSearchResultsQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await new SearchResultValidator().ValidateAsync(request, cancellationToken);

        return !validationResult.IsValid
            ? Result.Invalid<LocationFilterParameters>(validationResult.Errors)
            : (await _locationService.GetLocationFilterParametersAsync(request.Postcode!)).TryValidate();
    }

    private async Task<IEnumerable<TuitionPartnerResult>> FindTuitionPartners(
        LocationFilterParameters parameters,
        GetSearchResultsQuery request,
        CancellationToken cancellationToken)
    {
        var keyStageSubjects = request.Subjects?.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

        var subjects = await _lookupDataService.GetSubjectsAsync(cancellationToken);

        var subjectFilterIds = subjects
                                .Where(e => keyStageSubjects.Select(x => $"{x.KeyStage}-{x.Subject}".ToSeoUrl()).Contains(e.SeoUrl))
                                .Select(x => x.Id);

        var tuitionFilterId = request.TuitionType > 0 ? (int?)request.TuitionType : null;

        var tuitionPartnersIds = await _tuitionPartnerService.GetTuitionPartnersFilteredAsync(
            new TuitionPartnersFilter
            {
                LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
                SubjectIds = subjectFilterIds,
                TuitionTypeId = tuitionFilterId
            }, cancellationToken);

        var tuitionPartners = await _tuitionPartnerService.GetTuitionPartnersAsync(new TuitionPartnerRequest
        {
            TuitionPartnerIds = tuitionPartnersIds,
            LocalAuthorityDistrictId = parameters.LocalAuthorityDistrictId,
            Urn = parameters.Urn
        }, cancellationToken);

        tuitionPartners = _tuitionPartnerService.OrderTuitionPartners(tuitionPartners, new TuitionPartnerOrdering
        {
            OrderBy = TuitionPartnerOrderBy.Random,
            Direction = OrderByDirection.Ascending,
            RandomSeed = TuitionPartnerOrdering.RandomSeedGeneration(parameters.LocalAuthorityDistrictCode,
                parameters.Postcode, subjectFilterIds, tuitionFilterId)
        });

        return tuitionPartners;
    }
}