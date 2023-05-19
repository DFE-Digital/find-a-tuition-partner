namespace Domain;

public class TuitionSetting
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<Enquiry>? Enquiries { get; set; } = null!;

    protected bool Equals(TuitionSetting other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TuitionSetting)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}