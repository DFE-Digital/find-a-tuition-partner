namespace Application.Common.Interfaces;

public interface ISessionService
{
    void InitSession();
    Task AddOrUpdateDataAsync(string sessionIdKey, Dictionary<string, string> data);
    Task<Dictionary<string, string>?> RetrieveDataAsync(string sessionIdKey);
    Task DeleteDataAsync(string sessionIdKey);
}