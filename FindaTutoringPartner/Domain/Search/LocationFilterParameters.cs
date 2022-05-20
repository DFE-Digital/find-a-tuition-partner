namespace Domain.Search;

public class LocationFilterParameters
{
    public string Postcode { get; set; } = null!;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string Country { get; set; } = null!;
    public string? Region { get; set; }
    public string LocalAuthorityCode { get; set; } = null!;
    public string LocalAuthority { get; set; } = null!;
}