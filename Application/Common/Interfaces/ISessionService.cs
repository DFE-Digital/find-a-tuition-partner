namespace Application.Common.Interfaces;

public interface ISessionService
{
    private const string DefaultPreKey = "General";
    Task AddOrUpdateDataAsync(string key, string value, string preKey = DefaultPreKey);
    Task AddOrUpdateDataAsync(Dictionary<string, string> data, string preKey = DefaultPreKey);
    Task<string> RetrieveDataByKeyAsync(string key, string preKey = DefaultPreKey);
    Task<Dictionary<string, string>?> RetrieveDataAsync(string preKey = DefaultPreKey);
    Task DeleteDataAsync(string preKey = DefaultPreKey);
    Task<bool> SessionDataExistsAsync(string preKey = DefaultPreKey);
    Task<bool> AnySessionDataExistsAsync();
    Task ClearAllAsync();
    Task<bool> IsDuplicateFormPostAsync(string formPostTimestampKey = "FormPostTimestamp");
    Task StartFormPostProcessing(string formPostTimestampKey = "FormPostTimestamp");
    Task SetFormPostResponse<T>(T postResponseModel, string formPostModelKey = "FormPostModelKey");
    Task<T> GetPreviousFormPostResponse<T>(string formPostModelKey = "FormPostModelKey");
    Task Set<T>(string key, T value, string preKey = DefaultPreKey);
    Task<T?> Get<T>(string key, string preKey = DefaultPreKey);
}