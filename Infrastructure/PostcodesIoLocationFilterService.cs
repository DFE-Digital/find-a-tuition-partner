using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Application;
using Application.Repositories;
using Domain.Search;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class PostcodesIoLocationFilterService : ILocationFilterService
{
    private readonly HttpClient _httpClient;
    private readonly IGeographyLookupRepository _geographyLookupRepository;
    private readonly INtpDbContext _dbContext;

    public PostcodesIoLocationFilterService(HttpClient httpClient, IGeographyLookupRepository geographyLookupRepository, INtpDbContext dbContext)
    {
        _httpClient = httpClient;
        _geographyLookupRepository = geographyLookupRepository;
        _dbContext = dbContext;
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
            LocalAuthorityDistrict = result["admin_district"]?.ToString() ?? "",
            LocalAuthorityDistrictCode = result["codes"]!["admin_district"]!.ToString()
        };

        var lad = await _geographyLookupRepository.GetLocalAuthorityDistrictAsync(parameters.LocalAuthorityDistrictCode);
        if (lad == null) return parameters;

        parameters.LocalAuthorityCode = lad.LocalAuthority.Code;
        parameters.LocalAuthority = lad.LocalAuthority.Name;
        parameters.LocalAuthorityDistrictCode = lad.Code;
        parameters.LocalAuthorityDistrict = lad.Name;

        parameters.School = await _dbContext.GeneralInformationAboutSchools.FirstOrDefaultAsync(e => e.Postcode == parameters.Postcode);

        return parameters;
    }
}