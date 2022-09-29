namespace Domain.Search;

public class TuitionPartnerSearchResult
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Subject[] Subjects { get; set; } = null!;
    public TuitionType[] TuitionTypes { get; set; } = null!;
    public bool HasSenProvision { get; set; }
}