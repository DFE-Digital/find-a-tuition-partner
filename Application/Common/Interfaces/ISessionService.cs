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
    Task StartFormPostProcessingAsync(string formPostTimestampKey = "FormPostTimestamp");
    Task SetFormPostResponseAsync<T>(T postResponseModel, string formPostModelKey = "FormPostModelKey");
    Task<T> GetPreviousFormPostResponseAsync<T>(string formPostModelKey = "FormPostModelKey");
}