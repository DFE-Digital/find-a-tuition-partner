namespace Application.Common.Interfaces;

public interface ISessionService
{
    bool IsSessionAvailable();
    Task AddOrUpdateDataAsync(Dictionary<string, string> data);
    Task<Dictionary<string, string>?> RetrieveDataAsync();
    Task DeleteDataAsync();
}