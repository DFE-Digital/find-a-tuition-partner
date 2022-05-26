using System.Text.Json;
using Application.Exceptions;
using Application.Repositories;
using Domain.Search;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class SearchStateRepository : ISearchStateRepository
{
    private readonly NtpDbContext _dbContext;

    public SearchStateRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SearchState> CreateAsync()
    {
        var searchId = Guid.NewGuid();

        var state = new SearchState { SearchId = searchId };

        var userSearch = new UserSearch
        {
            Id = searchId,
            CreatedDate = DateTime.UtcNow,
            SearchJson = JsonSerializer.Serialize(state)
        };

        _dbContext.UserSearches.Add(userSearch);

        await _dbContext.SaveChangesAsync();

        return state;
    }

    public async Task<SearchState> RetrieveAsync(Guid searchId)
    {
        var userSearch = await _dbContext.UserSearches.FindAsync(searchId);

        if (userSearch == null) throw new SearchStateNotFoundException(searchId);

        var state = JsonSerializer.Deserialize<SearchState>(userSearch.SearchJson);

        if (state == null) throw new SearchStateNotFoundException(searchId);

        return state;
    }

    public async Task<SearchState> UpdateAsync(SearchState state)
    {
        var searchId = state.SearchId;

        var userSearch = await _dbContext.UserSearches.FindAsync(searchId);

        if (userSearch == null)
        {
            userSearch = new UserSearch
            {
                Id = searchId,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.UserSearches.Add(userSearch);
        }

        userSearch.UpdatedDate = DateTime.UtcNow;
        userSearch.SearchJson = JsonSerializer.Serialize(state);

        await _dbContext.SaveChangesAsync();

        return state;
    }

    public async Task DeleteAsync(Guid searchId)
    {
        var userSearch = await _dbContext.UserSearches.FindAsync(searchId);

        if (userSearch != null)
        {
            _dbContext.UserSearches.Remove(userSearch);
            await _dbContext.SaveChangesAsync();
        }
    }
}