using Application.Common.DTO;
using Application.Common.Interfaces;
using Domain.Exceptions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.ApiClients;

public class OneDriveApiClient : IOneDriveApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OneDriveSettings _config;
    private readonly ILogger<OneDriveApiClient> _logger;

    public OneDriveApiClient(IOptions<OneDriveSettings> config, IHttpClientFactory httpClientFactory,
        ILogger<OneDriveApiClient> logger)
    {
        _logger = logger;
        _config = config.Value;
        _httpClient = httpClientFactory.CreateClient(nameof(OneDriveApiClient));
    }

    public async Task<List<DriveItem>> GetFilesInFolder(string folderId, string[]? fileExtensions = null)
    {
        var endpoint = $"v1.0/drives/{_config.SharedDriveId}/items/{folderId}/children";

        _logger.LogInformation("Making a request to OneDrive API to get files in a folder: {endpoint}", endpoint);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new OneDriveApiClientException(
                    $"Failed to get files in folder {folderId}: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DriveItemCollectionResponse>(json);

            _logger.LogInformation("Successfully got files in folder {folderId}", folderId);

            if (fileExtensions != null && fileExtensions.Length == 1 && result != null)
            {
                return result.Value.Where(x => x.Name.EndsWith(fileExtensions.First())).ToList();
            }

            if (fileExtensions != null && fileExtensions.Length == 2 && result != null)
            {
                return result.Value.Where(x => x.Name.EndsWith(fileExtensions[0]) || x.Name.EndsWith(fileExtensions[1]))
                    .ToList();
            }

            return result != null ? result.Value.ToList() : new List<DriveItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get files in folder {folderId}", folderId);
            throw;
        }
    }

    public async Task<Stream> DownloadFile(string fileId, string fileName)
    {
        var endpoint = $"v1.0/drives/{_config.SharedDriveId}/items/{fileId}/content";

        _logger.LogInformation("Making a request to OneDrive API to download file: {endpoint}", endpoint);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new OneDriveApiClientException(
                    $"Failed to download file {fileId}-{fileName}: {response.ReasonPhrase}");
            }

            var stream = await response.Content.ReadAsStreamAsync();

            _logger.LogInformation("Got response from OneDrive API to download file: {fileId}-{fileName}", fileId,
                fileName);

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {fileId}-{fileName}", fileId, fileName);
            throw;
        }
    }
}