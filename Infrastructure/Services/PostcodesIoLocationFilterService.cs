﻿using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Application.Common.Interfaces;
using Domain;
using Domain.Search;

namespace Infrastructure.Services;

public class PostcodesIoLocationFilterService : ILocationFilterService
{
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;

    public PostcodesIoLocationFilterService(HttpClient httpClient, IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
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
            LocalAuthorityDistrictCode = result["codes"]!["admin_district"]!.ToString(),
            LocalAdministrativeUnit2Code = result["codes"]!["lau2"]!.ToString()
        };

        LocalAuthorityDistrict? lad = null;

        //Use the "lau2" to get the LAD, since this is more granular and matches the dataset supplied by TPs
        if (!string.IsNullOrEmpty(parameters.LocalAdministrativeUnit2Code))
        {
            lad = await _unitOfWork.LocalAuthorityDistrictRepository.GetLocalAuthorityDistrictAsync(parameters.LocalAdministrativeUnit2Code);

            //If there was an lad match then check that the "lau2" and "admin_district" have the same LA Id
            if (lad != null &&
                !string.IsNullOrEmpty(parameters.LocalAuthorityDistrictCode) &&
                !parameters.LocalAdministrativeUnit2Code.Equals(parameters.LocalAuthorityDistrictCode, StringComparison.InvariantCultureIgnoreCase))
            {
                var ladToCompare = await _unitOfWork.LocalAuthorityDistrictRepository.GetLocalAuthorityDistrictAsync(parameters.LocalAuthorityDistrictCode);
                if (ladToCompare != null && ladToCompare.LocalAuthorityId != lad.LocalAuthorityId)
                {
                    lad = null;
                }
            }
        }

        //If no lad from "lau2" (above) then use "admin_district"
        lad ??= await _unitOfWork.LocalAuthorityDistrictRepository.GetLocalAuthorityDistrictAsync(parameters.LocalAuthorityDistrictCode);

        if (lad == null)
        {
            return parameters;
        }

        parameters.LocalAuthorityCode = lad.LocalAuthority.Code;
        parameters.LocalAuthority = lad.LocalAuthority.Name;
        parameters.LocalAuthorityId = lad.LocalAuthority.Id;
        parameters.LocalAuthorityDistrictId = lad.Id;
        parameters.LocalAuthorityDistrictCode = lad.Code;
        parameters.LocalAuthorityDistrict = lad.Name;
        var schools = await _unitOfWork.SchoolRepository.GetAllAsync(e => e.Postcode == parameters.Postcode && e.IsActive, "PhaseOfEducation,LocalAuthority", false);
        if (schools != null && schools.Any())
        {
            parameters.Schools = schools.ToList();
            parameters.Urn = schools.First().Urn;
        }

        return parameters;
    }
}