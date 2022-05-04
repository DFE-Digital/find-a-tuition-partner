namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        Addresses = new List<Address>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; }
}