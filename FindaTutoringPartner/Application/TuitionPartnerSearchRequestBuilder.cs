using Application.Exceptions;
using Application.Repositories;
using Domain.Search;

namespace Application;

public class TuitionPartnerSearchRequestBuilder
{
    private readonly ISearchStateRepository _searchStateRepository;
    private readonly ILocationFilterService _locationFilterService;

    public TuitionPartnerSearchRequestBuilder(SearchState searchState, ISearchStateRepository searchStateRepository, ILocationFilterService locationFilterService)
    {
        SearchState = searchState;
        _searchStateRepository = searchStateRepository;
        _locationFilterService = locationFilterService;
    }

    public Guid SearchId => SearchState.SearchId;
    public SearchState SearchState { get; private set; }

    public async Task<TuitionPartnerSearchRequestBuilder> WithPostcode(string? postcode)
    {
        if (postcode == null)
        {
            SearchState.LocationFilterParameters = null;
            return this;
        }

        var parameters = await _locationFilterService.GetLocationFilterParametersAsync(postcode);
        if (parameters == null)
        {
            throw new LocationNotFoundException();
        }

        SearchState.LocationFilterParameters = parameters;
        SearchState = await _searchStateRepository.UpdateAsync(SearchState);

        return this;
    }

    public TuitionPartnerSearchRequest Build()
    {
        return new TuitionPartnerSearchRequest();
    }
}