using Application;
using Application.Repositories;

namespace Infrastructure.Repositories;

public class SearchRequestBuilderRepository : ISearchRequestBuilderRepository
{
    private readonly ISearchStateRepository _searchStateRepository;
    private readonly ILocationFilterService _locationFilterService;

    public SearchRequestBuilderRepository(ISearchStateRepository searchStateRepository, ILocationFilterService locationFilterService)
    {
        _searchStateRepository = searchStateRepository;
        _locationFilterService = locationFilterService;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> CreateAsync()
    {
        var state = await _searchStateRepository.CreateAsync();

        var builder = new TuitionPartnerSearchRequestBuilder(state, _searchStateRepository, _locationFilterService);

        return builder;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> RetrieveAsync(Guid searchId)
    {
        var state = await _searchStateRepository.RetrieveAsync(searchId);

        var builder = new TuitionPartnerSearchRequestBuilder(state, _searchStateRepository, _locationFilterService);

        return builder;
    }
}