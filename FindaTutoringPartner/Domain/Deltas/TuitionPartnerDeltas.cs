namespace Domain.Deltas;

public class TuitionPartnerDeltas
{
    public TuitionPartnerDeltas()
    {
        Add = new List<TuitionPartner>();
        Update = new List<TuitionPartnerDelta>();
        Remove = new List<TuitionPartner>();
    }

    public ICollection<TuitionPartner> Add { get; set; }
    public ICollection<TuitionPartnerDelta> Update { get; set; }
    public ICollection<TuitionPartner> Remove { get; set; }
}