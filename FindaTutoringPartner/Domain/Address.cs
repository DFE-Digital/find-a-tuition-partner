using System.Text;

namespace Domain;

public class Address
{
    public int Id { get; set; }
    public string Line1 { get; set; } = null!;
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    public string? Line4 { get; set; }
    public string Postcode { get; set; } = null!;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder($"{Line1} ");

        if(!string.IsNullOrEmpty(Line2))
        {
            sb.Append($"{Line2} ");
        }
        if(!string.IsNullOrEmpty(Line3))
        {
            sb.Append($"{Line3} ");
        }
        if(!string.IsNullOrEmpty(Line4))
        {
            sb.Append($"{Line4} ");
        }
        
        sb.Append(Postcode);

        return sb.ToString().Trim();
    }
}