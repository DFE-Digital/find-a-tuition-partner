using System.Text;
using Application.Common.Interfaces;
using Newtonsoft.Json;
using Polly;
using IntegerConstants = UI.Constants.IntegerConstants;

namespace UI.Services;

public class DistributedSessionService : ISessionService
{
    private const string DefaultPreKey = "General";
    public const string FormPostPreKey = "FormPostPreKey";

    private readonly IHttpContextAccessor? _contextAccessor;
    private readonly ILogger<DistributedSessionService> _logger;

    public DistributedSessionService(IHttpContextAccessor contextAccessor, ILogger<DistributedSessionService> logger)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException($"{nameof(contextAccessor)}");
        _logger = logger;
    }

    public async Task AddOrUpdateDataAsync(string key, string value, string preKey = DefaultPreKey)
    {
        await AddOrUpdateDataAsync(new Dictionary<string, string>()
        {
            { key, value}
        },
        preKey);
    }

    public async Task AddOrUpdateDataAsync(Dictionary<string, string> data, string preKey = DefaultPreKey)
    {
        if (IsSessionAvailable())
        {
            await LoadDataFromDistributedDataStore();

            var dataString = JsonConvert.SerializeObject(data);

            var sessionKey = GetSessionKey(preKey);

            var storedValue = GetString(sessionKey);

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
                SetString(sessionKey, updatedValue);
            }
            else
            {
                // Add
                SetString(sessionKey, dataString);
            }

            await CommitDataToDistributedDataStore();
        }
    }

    public async Task<string> RetrieveDataByKeyAsync(string key, string preKey = DefaultPreKey)
    {
        var result = string.Empty;

        if (string.IsNullOrEmpty(key)) return result;

        var sessionValues = await RetrieveDataAsync(preKey);

        if (sessionValues?.TryGetValue(key, out var value) == true)
        {
            result = value;
        }

        return result;
    }
    public async Task<Dictionary<string, string>?> RetrieveDataAsync(string preKey = DefaultPreKey)
    {
        if (!IsSessionAvailable()) return null;
        await LoadDataFromDistributedDataStore();
        var sessionKey = GetSessionKey(preKey);
        var storedValue = GetString(sessionKey);
        return storedValue == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(storedValue);

    }

    public async Task DeleteDataAsync(string preKey = DefaultPreKey)
    {
        if (IsSessionAvailable())
        {
            await LoadDataFromDistributedDataStore();
            var sessionKey = GetSessionKey(preKey);
            var storedValue = GetString(sessionKey);
            if (storedValue != null)
            {
                _contextAccessor!.HttpContext!.Session!.Remove(sessionKey);
            }
        }
    }

    public async Task<bool> SessionDataExistsAsync(string preKey = DefaultPreKey)
    {
        await LoadDataFromDistributedDataStore();
        var sessionKey = GetSessionKey(preKey);
        var sessionExists = GetString(sessionKey) != null;
        if (!sessionExists)
        {
            _logger.LogInformation("Session expired or empty with key: {preKey}", preKey);
        }
        return sessionExists;
    }

    public async Task<bool> AnySessionDataExistsAsync()
    {
        await LoadDataFromDistributedDataStore();
        return _contextAccessor!.HttpContext!.Session!.Keys.Where(x => x != FormPostPreKey).Any();
    }

    public async Task ClearAllAsync()
    {
        await LoadDataFromDistributedDataStore();
        _logger.LogInformation("Session cleared");
        _contextAccessor!.HttpContext!.Session!.Clear();
    }

    public async Task<bool> IsDuplicateFormPostAsync(string formPostTimestampKey = "FormPostTimestamp")
    {
        var previousSubmissionTimestamp = await GetAsync<DateTimeOffset?>(formPostTimestampKey, FormPostPreKey);

        var currentTimestamp = DateTimeOffset.UtcNow;

        if (previousSubmissionTimestamp != null)
        {
            if ((currentTimestamp - previousSubmissionTimestamp.Value).TotalSeconds < IntegerConstants.SecondsClassifyAsDuplicateSubmission)
            {
                _logger.LogInformation("Multiple enquiry submission");
                return true;
            }
        }

        return false;
    }

    public async Task StartFormPostProcessingAsync(string formPostTimestampKey = "FormPostTimestamp")
    {
        var currentTimestamp = DateTimeOffset.UtcNow;
        await SetAsync(formPostTimestampKey, currentTimestamp, FormPostPreKey);
    }

    public async Task SetFormPostResponseAsync<T>(T postResponseModel, string formPostModelKey = "FormPostModelKey")
    {
        await SetAsync(formPostModelKey, postResponseModel, FormPostPreKey);
    }

    public async Task<T> GetPreviousFormPostResponseAsync<T>(string formPostModelKey = "FormPostModelKey")
    {
        //Use Polly to retry
        int numberOfRetries = IntegerConstants.SecondsClassifyAsDuplicateSubmission;
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(numberOfRetries, retryAttempt =>
                TimeSpan.FromSeconds(1),
                (exception, sleepDuration, retryCount, context) =>
                {
                    _logger.LogInformation("GetPreviousPostResponse - Not able to get previous form POST response info.  Retrying in {SleepDuration}. Attempt {RetryCount} out of {NumberOfRetries}", sleepDuration, retryCount, numberOfRetries);
                });

        _logger.LogInformation("GetPreviousPostResponse - trying to get previous form POST response info");
        try
        {
            return await retryPolicy.ExecuteAsync(async () =>
            {
                var submittedConfirmationModelFromSession = await GetAsync<T?>(formPostModelKey, FormPostPreKey);

                if (submittedConfirmationModelFromSession != null)
                {
                    _logger.LogInformation("GetPreviousPostResponse - returning previous SubmittedConfirmationModel from session");
                    return submittedConfirmationModelFromSession;
                }
                else
                {
                    throw new InvalidDataException("Not able to get previous form POST response info");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPreviousPostResponse - Not able to get previous form POST response info");
            throw;
        }
    }

    public async Task SetAsync<T>(string key, T value, string preKey = DefaultPreKey)
    {
        await AddOrUpdateDataAsync(key, JsonConvert.SerializeObject(value), preKey);
    }

    public async Task<T?> GetAsync<T>(string key, string preKey = DefaultPreKey)
    {
        var value = await RetrieveDataByKeyAsync(key, preKey);
        return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value);
    }

    private bool IsSessionAvailable()
    {
        var isSessionAvailable = _contextAccessor!.HttpContext != null && _contextAccessor!.HttpContext.Session.IsAvailable;
        if (!isSessionAvailable)
        {
            _logger.LogError("Session unavailable");
        }
        return isSessionAvailable;
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

    private string GetSessionKey(string preKey)
    {
        return $"{preKey}_{_contextAccessor!.HttpContext!.Session.Id}";
    }
}