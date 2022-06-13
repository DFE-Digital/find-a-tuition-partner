using Application;
using Application.Repositories;

namespace Infrastructure.Repositories;

public class SearchRequestBuilderRepository : ISearchRequestBuilderRepository
{
    private readonly ISearchStateRepository _searchStateRepository;
    private readonly ILocationFilterService _locationFilterService;
    private readonly IGeographyLookupRepository _geographyLookupRepository;
    private readonly ILookupDataRepository _lookupDataRepository;

    public SearchRequestBuilderRepository(ISearchStateRepository searchStateRepository, ILocationFilterService locationFilterService, IGeographyLookupRepository geographyLookupRepository, ILookupDataRepository lookupDataRepository)
    {
        _searchStateRepository = searchStateRepository;
        _locationFilterService = locationFilterService;
        _geographyLookupRepository = geographyLookupRepository;
        _lookupDataRepository = lookupDataRepository;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> CreateAsync()
    {
        var state = await _searchStateRepository.CreateAsync();

        var builder = new TuitionPartnerSearchRequestBuilder(state, _searchStateRepository, _locationFilterService, _geographyLookupRepository, _lookupDataRepository);

        return builder;
    }

    public async Task<TuitionPartnerSearchRequestBuilder> RetrieveAsync(Guid searchId)
    {
        var state = await _searchStateRepository.RetrieveAsync(searchId);

        var builder = new TuitionPartnerSearchRequestBuilder(state, _searchStateRepository, _locationFilterService, _geographyLookupRepository, _lookupDataRepository);

        return builder;
    }
}