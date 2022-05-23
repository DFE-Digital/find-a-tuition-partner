namespace Domain.Deltas;

public class TuitionPartnerDeltas
{
    public TuitionPartnerDeltas()
    {
        Add = new List<TuitionPartner>();
        Update = new List<TuitionPartner>();
        Remove = new List<TuitionPartner>();
    }

    public ICollection<TuitionPartner> Add { get; set; }
    public ICollection<TuitionPartner> Update { get; set; }
    public ICollection<TuitionPartner> Remove { get; set; }
}