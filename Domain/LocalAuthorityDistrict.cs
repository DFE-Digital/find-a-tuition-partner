namespace Domain;

public  class LocalAuthorityDistrict
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int RegionId { get; set; } 
    public Region Region { get; set; } = null!;
}