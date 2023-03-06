using System.Text;
using Application.Common.Interfaces;
using Newtonsoft.Json;

namespace UI.Services;

public class DistributedSessionService : ISessionService
{
    private readonly IHttpContextAccessor? _contextAccessor;

    public DistributedSessionService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException($"{nameof(contextAccessor)}");
    }

    public async Task AddOrUpdateDataAsync(string key, string value)
    {
        await AddOrUpdateDataAsync(new Dictionary<string, string>()
        {
            { key, value}
        });
    }

    public async Task AddOrUpdateDataAsync(Dictionary<string, string> data)
    {
        if (IsSessionAvailable())
        {
            await LoadDataFromDistributedDataStore();

            var dataString = JsonConvert.SerializeObject(data);

            var storedValue = GetString(_contextAccessor!.HttpContext!.Session.Id);

            if (!string.IsNullOrEmpty(storedValue))
            {
                var existingData = JsonConvert.DeserializeObject<Dictionary<string, string>>(storedValue);

                // Update existing data with new values
                foreach (var key in data.Keys)
                {
                    if (existingData!.ContainsKey(key))
                    {
                        existingData[key] = data[key];
                    }
                    else
                    {
                        existingData.Add(key, data[key]);
                    }
                }

                var updatedValue = JsonConvert.SerializeObject(existingData);
                SetString(_contextAccessor!.HttpContext!.Session.Id, updatedValue);
            }
            else
            {
                // Add
                SetString(_contextAccessor!.HttpContext!.Session.Id, dataString);
            }

            await CommitDataToDistributedDataStore();
        }
    }

    public async Task<string> RetrieveDataAsync(string key)
    {
        var result = string.Empty;

        if (string.IsNullOrEmpty(key)) return result;

        var sessionValues = await RetrieveDataAsync();

        if (sessionValues?.TryGetValue(key, out var value) == true)
        {
            result = value;
        }

        return result;
    }
    public async Task<Dictionary<string, string>?> RetrieveDataAsync()
    {
        if (!IsSessionAvailable()) return null;
        await LoadDataFromDistributedDataStore();
        var storedValue = GetString(_contextAccessor!.HttpContext!.Session.Id);
        return storedValue == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(storedValue);

    }

    public async Task DeleteDataAsync()
    {
        if (IsSessionAvailable())
        {
            await LoadDataFromDistributedDataStore();
            var storedValue = GetString(_contextAccessor!.HttpContext!.Session.Id);
            if (storedValue != null)
            {
                _contextAccessor!.HttpContext!.Session!.Remove(_contextAccessor!.HttpContext!.Session.Id);
            }
        }
    }

    private bool IsSessionAvailable()
    {
        return _contextAccessor!.HttpContext != null && _contextAccessor!.HttpContext.Session.IsAvailable;
    }

    public async Task<bool> SessionDataExistsAsync()
    {
        await LoadDataFromDistributedDataStore();
        return _contextAccessor!.HttpContext!.Session!.Keys.Any();
    }

    public async Task ClearAsync()
    {
        await LoadDataFromDistributedDataStore();
        _contextAccessor!.HttpContext!.Session!.Clear();
    }

    private async Task LoadDataFromDistributedDataStore()
    {
        await _contextAccessor!.HttpContext!.Session!.LoadAsync();
    }

    private async Task CommitDataToDistributedDataStore()
    {
        await _contextAccessor!.HttpContext!.Session!.CommitAsync();
    }

    private string? GetString(string sessionIdKey)
    {
        var data = _contextAccessor!.HttpContext!.Session!.Get(sessionIdKey);
        return data == null ? null : Encoding.UTF8.GetString(data);
    }

    private void SetString(string key, string value)
    {
        _contextAccessor!.HttpContext!.Session!.Set(key, Encoding.UTF8.GetBytes(value));
    }
}