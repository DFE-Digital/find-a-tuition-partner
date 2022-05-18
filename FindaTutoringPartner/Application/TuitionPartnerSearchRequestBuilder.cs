using Application.Exceptions;
using Application.Repositories;
using Domain.Search;

namespace Application;

public class TuitionPartnerSearchRequestBuilder
{
    private readonly ISearchStateRepository _searchStateRepository;
    private readonly ILocationFilterService _locationFilterService;

    public TuitionPartnerSearchRequestBuilder(SearchState state, ISearchStateRepository searchStateRepository, ILocationFilterService locationFilterService)
    {
        State = state;
        _searchStateRepository = searchStateRepository;
        _locationFilterService = locationFilterService;
    }

    public SearchState State { get; private set; }

    public async Task<TuitionPartnerSearchRequestBuilder> WithPostcode(string? postcode)
    {
        if (postcode == null)
        {
            State.LocationFilterParameters = null;
            return this;
        }

        State.LocationFilterParameters = await _locationFilterService.GetLocationFilterParametersAsync(postcode);

        if (State.LocationFilterParameters == null)
        {
            throw new LocationNotFoundException();
        }

        State = await _searchStateRepository.UpdateAsync(State);

        return this;
    }

    public TuitionPartnerSearchRequest Build()
    {
        return new TuitionPartnerSearchRequest();
    }
}