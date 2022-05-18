using Application.Exceptions;
using Domain.Search;

namespace Application;

public class TuitionPartnerSearchRequestBuilder
{
    private readonly ILocationFilterService _locationFilterService;

    public TuitionPartnerSearchRequestBuilder(TuitionPartnerSearchRequestBuilderState state, ILocationFilterService locationFilterService)
    {
        State = state;
        _locationFilterService = locationFilterService;
    }

    public TuitionPartnerSearchRequestBuilderState State { get; }

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

        return this;
    }

    public TuitionPartnerSearchRequest Build()
    {
        return new TuitionPartnerSearchRequest();
    }
}

public class TuitionPartnerSearchRequestBuilderState
{
    public Guid SearchId { get; set; }
    public LocationFilterParameters? LocationFilterParameters { get; set; }
}