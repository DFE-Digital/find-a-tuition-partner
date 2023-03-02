namespace Application.Common.Interfaces;

public interface ISessionService
{
    Task AddOrUpdateDataAsync(string key, string? value);
    Task AddOrUpdateDataAsync(Dictionary<string, string> data);
    Task<string?> RetrieveDataAsync(string key);
    Task<bool> SessionDataExistsAsync();
    Task ClearAsync();
}