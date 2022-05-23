using System.Globalization;
using Application;
using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class RegionalTuitionPartnerDataExtractor : ITuitionPartnerDataExtractor
{
    private const string RegionInitialsAll = "ALL";
    private const string RegionInitialsNorthEast = "NE";
    private const string RegionInitialsNorthWest = "NW";
    private const string RegionInitialsYorkshireandTheHumber = "YH";
    private const string RegionInitialsEastMidlands = "EM";
    private const string RegionInitialsWestMidlands = "WM";
    private const string RegionInitialsEastofEngland = "E";
    private const string RegionInitialsLondon = "L";
    private const string RegionInitialsSouthEast = "SE";
    private const string RegionInitialsSouthWest = "SW";

    private static readonly string[] AllRegionInitials = {
        RegionInitialsNorthEast,
        RegionInitialsNorthWest,
        RegionInitialsYorkshireandTheHumber,
        RegionInitialsEastMidlands,
        RegionInitialsWestMidlands,
        RegionInitialsEastofEngland,
        RegionInitialsLondon,
        RegionInitialsSouthEast,
        RegionInitialsSouthWest
    };

    private readonly NtpDbContext _dbContext;

    public RegionalTuitionPartnerDataExtractor(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(FileInfo csvFile, int tuitionTypeId)
    {
        var initialsToRegionDictionary = await GetInitialsToRegionDictionary();

        var subjects = await _dbContext.Subjects.OrderBy(e => e.Id)
            .ToDictionaryAsync(e => e.Id);

        var tuitionType = await _dbContext.TuitionTypes.SingleAsync(e => e.Id == tuitionTypeId);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            ShouldSkipRecord = args => new[] { "", "Face-to-Face" , "Online", "Tuition Partner" }.Contains(args.Record[0]),
            TrimOptions = TrimOptions.Trim
        };

        using var reader = new StreamReader(csvFile.OpenRead());
        using var csv = new CsvReader(reader, config);

        var data = csv.GetRecordsAsync<RegionalTuitionPartnerDatum>();

        await foreach (var datum in data)
        {
            var tuitionPartner = new TuitionPartner
            {
                Name = datum.Name,
                Website = ""
            };

            AddSubjectCoverage(tuitionPartner, datum.PrimaryLiteracyRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.PrimaryLiteracy], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.PrimaryNumeracyRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.PrimaryNumeracy], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.PrimaryScienceRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.PrimaryScience], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryEnglishRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.SecondaryEnglish], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryHumanitiesRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.SecondaryHumanities], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryMathsRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.SecondaryMaths], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryModernForeignLanguagesRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.SecondaryModernForeignLanguages], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryScienceRegionInitials, initialsToRegionDictionary, subjects[Subjects.Id.SecondaryScience], tuitionType);

            yield return tuitionPartner;
        }
    }

    private async Task<IDictionary<string, Region>> GetInitialsToRegionDictionary()
    {
        var regionsAndLads = await _dbContext.Regions
            .Include(e => e.LocalAuthorityDistricts.OrderBy(ce => ce.Code))
            .OrderBy(e => e.Id)
            .ToDictionaryAsync(e => e.Id);

        return new Dictionary<string, Region>
        {
            { RegionInitialsNorthEast, regionsAndLads[Regions.Id.NorthEast] },
            { RegionInitialsNorthWest, regionsAndLads[Regions.Id.NorthWest] },
            { RegionInitialsYorkshireandTheHumber, regionsAndLads[Regions.Id.YorkshireandTheHumber] },
            { RegionInitialsEastMidlands, regionsAndLads[Regions.Id.EastMidlands] },
            { RegionInitialsWestMidlands, regionsAndLads[Regions.Id.WestMidlands] },
            { RegionInitialsEastofEngland, regionsAndLads[Regions.Id.EastofEngland] },
            { RegionInitialsLondon, regionsAndLads[Regions.Id.London] },
            { RegionInitialsSouthEast, regionsAndLads[Regions.Id.SouthEast] },
            { RegionInitialsSouthWest, regionsAndLads[Regions.Id.SouthWest] }
        };
    }

    private void AddSubjectCoverage(TuitionPartner tuitionPartner, string? subjectRegionInitialsString, IDictionary<string, Region> initialsToRegionDictionary, Subject subject, TuitionType tuitionType)
    {
        if (string.IsNullOrEmpty(subjectRegionInitialsString)) return;

        var subjectRegionInitials = subjectRegionInitialsString.Split(',').Select(s => s.Trim().ToUpper()).ToArray();

        foreach (var regionInitial in AllRegionInitials)
        {
            if (!subjectRegionInitials.Contains(regionInitial) && !subjectRegionInitials.Contains(RegionInitialsAll)) continue;

            foreach (var localAuthorityDistrict in initialsToRegionDictionary[regionInitial].LocalAuthorityDistricts)
            {
                tuitionPartner.Coverage.Add(new TuitionPartnerCoverage
                {
                    TuitionPartner = tuitionPartner,
                    LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                    LocalAuthorityDistrict = localAuthorityDistrict,
                    SubjectId = subject.Id,
                    Subject = subject,
                    TuitionTypeId = tuitionType.Id,
                    TuitionType = tuitionType
                });
            }
        }
    }

    private class RegionalTuitionPartnerDatum
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string Name { get; set; } = null!;
        [CsvHelper.Configuration.Attributes.Index(1)]
        public string? PrimaryLiteracyRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(2)]
        public string? PrimaryNumeracyRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(3)]
        public string? PrimaryScienceRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(4)]
        public string? SecondaryEnglishRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(5)]
        public string? SecondaryHumanitiesRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(6)]
        public string? SecondaryMathsRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(7)]
        public string? SecondaryModernForeignLanguagesRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(8)]
        public string? SecondaryScienceRegionInitials { get; set; }
    }
}