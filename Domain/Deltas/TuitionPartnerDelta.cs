namespace Domain.Deltas;

public class TuitionPartnerDelta
{
    public TuitionPartnerDelta()
    {
        CoverageAdd = new List<TuitionPartnerCoverage>();
        CoverageUpdate = new List<TuitionPartnerCoverage>();
        CoverageRemove = new List<TuitionPartnerCoverage>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public ICollection<TuitionPartnerCoverage> CoverageAdd { get; set; }
    public ICollection<TuitionPartnerCoverage> CoverageUpdate { get; set; }
    public ICollection<TuitionPartnerCoverage> CoverageRemove { get; set; }
}