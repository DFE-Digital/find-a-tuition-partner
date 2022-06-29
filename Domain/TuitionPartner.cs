namespace Domain;

public class TuitionPartner
{
    public TuitionPartner()
    {
        Coverage = new List<TuitionPartnerCoverage>();
        Prices = new List<Price>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<TuitionPartnerCoverage> Coverage { get; set; }
    public ICollection<Price> Prices { get; set; }
}