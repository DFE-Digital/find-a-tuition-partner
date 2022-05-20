using Domain.Search;

namespace Application.Repositories;

public interface ISearchStateRepository
{
    Task<SearchState> CreateAsync();
    Task<SearchState> RetrieveAsync(Guid searchId);
    Task<SearchState> UpdateAsync(SearchState state);
    Task DeleteAsync(Guid searchId);
}