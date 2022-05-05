namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        Locations = new List<TuitionPartnerLocation>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public ICollection<TuitionPartnerLocation> Locations { get; set; }
}