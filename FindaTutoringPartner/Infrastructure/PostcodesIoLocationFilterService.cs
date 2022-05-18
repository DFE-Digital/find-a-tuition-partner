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

        if (content == null) return null;

        if (content["codes"]!["admin_district"] == null) return null;

        //https://postcodes.io/docs#Data
        var parameters = new LocationFilterParameters
        {
            Postcode = content["postcode"]!.ToString(),
            Longitude = content["longitude"] == null ? null : decimal.Parse(content["longitude"]!.ToString()),
            Latitude = content["latitude"] == null ? null : decimal.Parse(content["latitude"]!.ToString()),
            Country = content["country"]!.ToString(),
            Region = content["region"]?.ToString(),
            LocalAuthority = content["admin_district"]?.ToString() ?? "",
            LocalAuthorityCode = content["codes"]!["admin_district"]!.ToString()
        };

        return parameters;
    }
}