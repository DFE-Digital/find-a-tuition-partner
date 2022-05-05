namespace Domain;

public class TutorType
{
    public TutorType()
    {
        TuitionPartnerLocations = new List<TuitionPartnerLocation>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<TuitionPartnerLocation> TuitionPartnerLocations { get; set; }
}