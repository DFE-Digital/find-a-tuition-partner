using Application;

namespace Infrastructure;

public class SearchRequestBuilderRepository : ISearchRequestBuilderRepository
{
    private static readonly IDictionary<Guid, TuitionPartnerSearchRequestBuilder> _builders = new Dictionary<Guid, TuitionPartnerSearchRequestBuilder>();

    private readonly ILocationFilterService _locationFilterService;

    public SearchRequestBuilderRepository(ILocationFilterService locationFilterService)
    {
        _locationFilterService = locationFilterService;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> CreateAsync()
    {
        var state = new TuitionPartnerSearchRequestBuilderState { SearchId = Guid.NewGuid() };

        var builder = new TuitionPartnerSearchRequestBuilder(state, _locationFilterService);

        _builders[state.SearchId] = builder;

        return builder;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> RetrieveAsync(Guid searchId)
    {
        return _builders[searchId];
    }

    public async Task<TuitionPartnerSearchRequestBuilder> UpdateAsync(TuitionPartnerSearchRequestBuilder builder)
    {
        _builders[builder.State.SearchId] = builder;

        return builder;
    }

    public async Task DeleteAsync(Guid searchId)
    {
        _builders[searchId] = null;
    }
}