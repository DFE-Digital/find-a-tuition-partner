﻿using System.ComponentModel.DataAnnotations;

namespace Domain.Search;

public class LocationFilterParameters
{
    [Required(ErrorMessage = "Enter a postcode")]
    public string Postcode { get; set; } = null!;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string Country { get; set; } = null!;
    public string? Region { get; set; }
    public string LocalAuthorityDistrictCode { get; set; } = null!;
    public string LocalAuthorityDistrict { get; set; } = null!;
}