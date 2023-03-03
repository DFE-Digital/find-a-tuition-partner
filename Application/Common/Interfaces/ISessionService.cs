namespace Application.Common.Interfaces;

public interface ISessionService
{
    Task AddOrUpdateDataAsync(Dictionary<string, string> data);
    Task<Dictionary<string, string>?> RetrieveDataAsync();
    Task DeleteDataAsync();
}