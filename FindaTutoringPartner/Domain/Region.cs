namespace Domain;

public class Region
{
    public Region()
    {
        LocalAuthorityDistricts = new List<LocalAuthorityDistrict>();
    }

    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<LocalAuthorityDistrict> LocalAuthorityDistricts { get; set; } 
}