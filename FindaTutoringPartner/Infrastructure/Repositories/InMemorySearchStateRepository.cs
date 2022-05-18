using Application.Repositories;
using Domain.Search;

namespace Infrastructure.Repositories;

public class InMemorySearchStateRepository : ISearchStateRepository
{
    private static readonly IDictionary<Guid, SearchState> _states = new Dictionary<Guid, SearchState>();

    public async Task<SearchState> CreateAsync()
    {
        var state = new SearchState { SearchId = Guid.NewGuid() };

        _states[state.SearchId] = state;

        return state;
    }

    public async Task<SearchState> RetrieveAsync(Guid searchId)
    {
        return _states[searchId];
    }

    public async Task<SearchState> UpdateAsync(SearchState state)
    {
        _states[state.SearchId] = state;

        return state;
    }

    public async Task DeleteAsync(Guid searchId)
    {
        _states.Remove(searchId);
    }
}