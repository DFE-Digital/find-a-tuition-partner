using Domain;
using TuitionType = Domain.Enums.TuitionType;

namespace Tests.TestData;

public record TuitionPartnerBuilder
{
    private static int Ids = 1;
    public int Id { get; private init; } = Ids++;
    public string SeoName { get; private init; } = $"a-tuition-partner-{Ids}";
    public string Name { get; private init; } = "A Tuition Partner";
    public TuitionPartnerLogo? Logo { get; private init; }
    public string Description { get; private init; } = "A Tuition Partner Description";
    public string Website { get; private init; } = "https://website";
    public string PhoneNumber { get; private init; } = "phonenumber";
    public string EmailAddress { get; private init; } = "tp@example.com";
    public string PostalAddress { get; private set; } = "1 High Street\r\nBeautiful City\rThe County\nPostcode";
    public int OrganisationTypeId { get; private init; } = 1;
    public Dictionary<int, TuitionType[]> Districts { get; private init; } = new();
    public SubjectBuilder Subjects { get; private init; } = new SubjectBuilder();

    public static implicit operator TuitionPartner(TuitionPartnerBuilder builder) => new()
    {
        Id = builder.Id,
        SeoUrl = builder.SeoName,
        Name = builder.Name,
        Website = builder.Website,
        Description = builder.Description,
        PhoneNumber = builder.PhoneNumber,
        Email = builder.EmailAddress,
        Address = builder.PostalAddress,
        LocalAuthorityDistrictCoverage = builder.DistrictCoverage,
        SubjectCoverage = builder.Subjects.SubjectCoverage.Select(x => new { x.SubjectId, x.TuitionTypeId }).Distinct().Select(x => new SubjectCoverage() { SubjectId = x.SubjectId, TuitionTypeId = x.TuitionTypeId }).ToList(),
        Prices = builder.Subjects.Prices.Select(x => new { x.TuitionTypeId, x.SubjectId, x.GroupSize, x.HourlyRate }).Distinct().Select(x => new Price() { TuitionTypeId = x.TuitionTypeId, SubjectId = x.SubjectId, GroupSize = x.GroupSize, HourlyRate = x.HourlyRate }).ToList(),
        Logo = builder.Logo,
        OrganisationTypeId = builder.OrganisationTypeId,
    };

    public List<LocalAuthorityDistrictCoverage> DistrictCoverage =>
    Districts.SelectMany(district => district.Value.Select(tuition => new LocalAuthorityDistrictCoverage
    {
        LocalAuthorityDistrictId = district.Key,
        TuitionTypeId = (int)tuition,
    })).ToList();

    internal TuitionPartnerBuilder WithId(int id)
        => new TuitionPartnerBuilder(this) with { Id = id };

    internal TuitionPartnerBuilder WithName(string seoName, string? name = null)
        => new TuitionPartnerBuilder(this) with { Name = name ?? seoName, SeoName = seoName };

    internal TuitionPartnerBuilder WithDescription(string description)
        => new TuitionPartnerBuilder(this) with { Description = description };

    internal TuitionPartnerBuilder WithWebsite(string website)
        => new TuitionPartnerBuilder(this) with { Website = website };

    internal TuitionPartnerBuilder WithPhoneNumber(string phonenumber)
        => new TuitionPartnerBuilder(this) with { PhoneNumber = phonenumber };

    internal TuitionPartnerBuilder WithEmailAddress(string email)
        => new TuitionPartnerBuilder(this) with { EmailAddress = email };

    internal TuitionPartnerBuilder WithLogo(string logo, string extension = ".svg")
        => new TuitionPartnerBuilder(this) with
        {
            Logo = new()
            {
                Logo = logo,
                FileExtension = extension,
            }
        };

    internal TuitionPartnerBuilder TaughtIn(District district, params TuitionType[] tuition)
        => new TuitionPartnerBuilder(this) with
        {
            Districts = new Dictionary<int, TuitionType[]>(Districts)
            {
                [district.Id] = tuition.Any() ? tuition : new[] { TuitionType.InSchool }
            }
        };

    internal TuitionPartnerBuilder WithSubjects(Func<SubjectBuilder, SubjectBuilder> config)
        => new TuitionPartnerBuilder(this) with { Subjects = config(new SubjectBuilder()) };


    internal TuitionPartnerBuilder WithAddress(string address)
        => new TuitionPartnerBuilder(this) with { PostalAddress = address };
}
