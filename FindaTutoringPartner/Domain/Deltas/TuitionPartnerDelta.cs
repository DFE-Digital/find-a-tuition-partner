namespace Domain.Deltas;

public class TuitionPartnerDelta
{
    public TuitionPartnerDelta()
    {
        Add = new List<TuitionPartnerCoverage>();
        Remove = new List<TuitionPartnerCoverage>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public ICollection<TuitionPartnerCoverage> Add { get; set; }
    public ICollection<TuitionPartnerCoverage> Remove { get; set; }
}