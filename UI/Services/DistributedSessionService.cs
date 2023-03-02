using System.Text;
using Application.Common.Interfaces;

namespace UI.Services;

public class DistributedSessionService : ISessionService
{
    private readonly IHttpContextAccessor? _contextAccessor;

    public DistributedSessionService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException($"{nameof(contextAccessor)}");
    }

    public async Task AddOrUpdateDataAsync(string key, string? value)
    {
        await LoadDataFromDistributedDataStore();

        if (string.IsNullOrEmpty(value))
        {
            _contextAccessor!.HttpContext!.Session!.Remove(key);
        }
        else
        {
            SetString(key, value);
        }

        await CommitDataToDistributedDataStore();
    }

    public async Task AddOrUpdateDataAsync(Dictionary<string, string> data)
    {
        await LoadDataFromDistributedDataStore();

        foreach (var key in data.Keys)
        {
            SetString(key, data[key]);
        }

        await CommitDataToDistributedDataStore();
    }

    public async Task<string?> RetrieveDataAsync(string key)
    {
        await LoadDataFromDistributedDataStore();
        return GetString(key);
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

    private string? GetString(string key)
    {
        return _contextAccessor!.HttpContext!.Session!.GetString(key);
    }

    private void SetString(string key, string value)
    {
        _contextAccessor!.HttpContext!.Session!.SetString(key, value);
    }
}