﻿namespace Domain.Search;

public class TuitionPartnerResult
{
    public int Id { get; set; }
    public string SeoUrl { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Website { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public SubjectCoverage[] SubjectsCoverage { get; set; } = null!;
    public TuitionType[] TuitionTypes { get; set; } = null!;
    public Price[] Prices { get; set; } = null!;
    public bool HasSenProvision { get; set; }
    public bool HasLogo { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool IsVatCharged { get; set; }
    public string LegalStatus { get; set; } = string.Empty;
}