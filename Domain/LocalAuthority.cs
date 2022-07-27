namespace Domain;

public class LocalAuthority
{
    public LocalAuthority()
    {
        LocalAuthorityDistricts = new List<LocalAuthorityDistrict>();
    }

    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public ICollection<LocalAuthorityDistrict> LocalAuthorityDistricts { get; set; }
}