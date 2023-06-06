using Application.Queries;
using Domain.Constants;
using Domain.Enums;

namespace Tests.TestData;

public static class Basic
{
    public static GetSearchResultsQuery SearchResultsQuery => new()
    {
        KeyStages = new[] { KeyStage.KeyStage1 },
        Subjects = new[] { "KeyStage1-English" },
    };
}

public static class A
{
    public static TuitionPartnerBuilder TuitionPartner => new TuitionPartnerBuilder()
        .WithSubjects(s => s
            .Subject(Subjects.Id.KeyStage1English, l => l
                .FaceToFace()
                .Costing(12m)
                .ForGroupSizes(2)));

    public static Domain.School School => new Domain.School()
    {
        Id = 1,
        Urn = 10000,
        EstablishmentName = "A School",
        Address = "School address, some street",
        Postcode = "NE1 2ES",
        EstablishmentNumber = 1,
        IsActive = true,
        EstablishmentTypeGroup = new Domain.EstablishmentTypeGroup()
        {
            Id = 99999,
            Name = "EstablishmentTypeGroup A",
        },
        EstablishmentStatus = new Domain.EstablishmentStatus()
        {
            Id = 99999,
            Name = "EstablishmentStatus A",
        },
        PhaseOfEducation = new Domain.PhaseOfEducation()
        {
            Id = 99999,
            Name = "PhaseOfEducation A",
        },
        LocalAuthority = new Domain.LocalAuthority()
        {
            Id = 99999,
            Code = "LocalAuthority Code A",
            Name = "LocalAuthority Name A",
            Region = new Domain.Region()
            {
                Id = 99999,
                Code = "Region Code A",
                Name = "Region Name A",
            }
        },
        LocalAuthorityDistrict = new Domain.LocalAuthorityDistrict()
        {
            Id = 99999,
            Code = "LocalAuthorityDistrict Code A",
            Name = "LocalAuthorityDistrict Name A",
            RegionId = 99999,
            LocalAuthorityId = 99999,
        }
    };
}
