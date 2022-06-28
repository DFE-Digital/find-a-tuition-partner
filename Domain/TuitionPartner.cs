namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        Coverage = new List<TuitionPartnerCoverage>();

    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public ICollection<TuitionPartnerCoverage> Coverage { get; set; }
}