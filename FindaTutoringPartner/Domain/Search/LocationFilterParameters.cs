namespace Domain.Search;

public class LocationFilterParameters
{
    public string Postcode { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Country { get; set; }
    public string? Region { get; set; }
    public string LocalAuthorityCode { get; set; }
    public string LocalAuthority { get; set; }
}