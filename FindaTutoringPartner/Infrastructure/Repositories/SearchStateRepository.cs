using Application.Repositories;
using Domain.Search;

namespace Infrastructure.Repositories;

public class SearchStateRepository : ISearchStateRepository
{
    public async Task<SearchState> CreateAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<SearchState> RetrieveAsync(Guid searchId)
    {
        throw new NotImplementedException();
    }

    public async Task<SearchState> UpdateAsync(SearchState state)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid searchId)
    {
        throw new NotImplementedException();
    }
}