namespace Domain;

public class TuitionPartnerLocation
{
    public TuitionPartnerLocation()
    {
        Subjects = new List<Subject>();
        TutorTypes = new List<TutorType>();
    }

    public int Id { get; set; }
    public TuitionPartner TuitionPartner { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Address Address { get; set; } = null!;
    public int CoverageRadius { get; set; }
    public ICollection<Subject> Subjects { get; set; }
    public ICollection<TutorType> TutorTypes { get; set; }
}