using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Application;
using Domain.Search;

namespace Infrastructure;

public class PostcodesIoLocationFilterService : ILocationFilterService
{
    private readonly HttpClient _httpClient;

    public PostcodesIoLocationFilterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LocationFilterParameters?> GetLocationFilterParametersAsync(string postcode)
    {
        var response = await _httpClient.GetAsync($"postcodes/{postcode}");

        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (content == null || !content.TryGetPropertyValue("result", out var result) || result == null) return null;

        if (result["codes"]!["admin_district"] == null) return null;

        //https://postcodes.io/docs#Data
        var parameters = new LocationFilterParameters
        {
            Postcode = result["postcode"]!.ToString(),
            Longitude = result["longitude"] == null ? null : result["longitude"]!.GetValue<decimal>(),
            Latitude = result["latitude"] == null ? null : result["latitude"]!.GetValue<decimal>(),
            Country = result["country"]!.ToString(),
            Region = result["region"]?.ToString(),
            LocalAuthority = result["admin_district"]?.ToString() ?? "",
            LocalAuthorityCode = result["codes"]!["admin_district"]!.ToString()
        };

        return parameters;
    }
}