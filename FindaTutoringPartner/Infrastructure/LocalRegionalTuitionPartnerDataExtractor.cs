using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure;

public class LocalRegionalTuitionPartnerDataExtractor : ITuitionPartnerLocalRegionDataExtractor
{
    private readonly NtpDbContext _dbContext;
    public LocalRegionalTuitionPartnerDataExtractor(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(string fileName, int tuitionTypeId)
    {
        var initialsLocalAuthorityDistrictDictionary = await GetInitialsToLocalAuthorityDistrictDictionary();

        var subjects = await _dbContext.Subjects.OrderBy(e => e.Id)
          .ToDictionaryAsync(e => e.Id);

        var tuitionType = await _dbContext.TuitionTypes.SingleAsync(e => e.Id == tuitionTypeId);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            ShouldSkipRecord = args => new[] { "", "Face-to-Face", "Online", "Tuition Partner" }.Contains(args.Record[0]),
            TrimOptions = TrimOptions.Trim
        };
        await using var reader = typeof(AssemblyReference).Assembly.GetManifestResourceStream(fileName);
        using var csv = new CsvReader(new StreamReader(reader ?? throw new InvalidOperationException()), config);

        var data = csv.GetRecordsAsync<LocalRegionalTuitionPartnerDatum>();
        await foreach (var datum in data)
        {
            var tuitionPartner = new TuitionPartner
            {
                Name = datum.Name,
                Website = ""
            };
            AddSubjectCoverage(tuitionPartner, datum.PrimaryLiteracyLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.PrimaryLiteracy], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.PrimaryNumeracyLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.PrimaryNumeracy], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.PrimaryScienceLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.PrimaryScience], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryEnglishLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.SecondaryEnglish], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryHumanitiesLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.SecondaryHumanities], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryMathsLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.SecondaryMaths], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryModernForeignLanguagesLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.SecondaryModernForeignLanguages], tuitionType);
            AddSubjectCoverage(tuitionPartner, datum.SecondaryScienceLocalRegionDistricts, initialsLocalAuthorityDistrictDictionary, subjects[Subjects.Id.SecondaryScience], tuitionType);
            yield return tuitionPartner;
        }
    }
    private void AddSubjectCoverage(TuitionPartner tuitionPartner, string? subjectLocalRegionDistrictsString, IDictionary<string, LocalAuthorityDistrict> initialsToRegionDictionary, Subject subject, TuitionType tuitionType)
    {
        if (string.IsNullOrEmpty(subjectLocalRegionDistrictsString)) return;
        var subjectLocalRegionDistricts = subjectLocalRegionDistrictsString.Split(',').Select(s => s.Trim().ToUpper()).ToArray();

        foreach (var subjectLocalRegionDistrict in subjectLocalRegionDistricts)
        {
            var coverage = tuitionPartner.Coverage.SingleOrDefault(e => e.LocalAuthorityDistrict.Name == subjectLocalRegionDistrict && e.TuitionTypeId == tuitionType.Id);
            if (coverage == null)
            {
                var localAuthorityDistrict = initialsToRegionDictionary[subjectLocalRegionDistrict.ToLower()];
                coverage = new TuitionPartnerCoverage
                {
                    TuitionPartnerId = tuitionPartner.Id,
                    TuitionPartner = tuitionPartner,
                    LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                    LocalAuthorityDistrict = localAuthorityDistrict,
                    TuitionType = tuitionType,
                    TuitionTypeId = tuitionType.Id
                };
            }
            var covered = subjectLocalRegionDistricts.Contains(subjectLocalRegionDistrict);
            switch (subject.Id)
            {
                case Subjects.Id.PrimaryLiteracy: coverage.PrimaryLiteracy = covered; break;
                case Subjects.Id.PrimaryNumeracy: coverage.PrimaryNumeracy = covered; break;
                case Subjects.Id.PrimaryScience: coverage.PrimaryScience = covered; break;
                case Subjects.Id.SecondaryEnglish: coverage.SecondaryEnglish = covered; break;
                case Subjects.Id.SecondaryHumanities: coverage.SecondaryHumanities = covered; break;
                case Subjects.Id.SecondaryMaths: coverage.SecondaryMaths = covered; break;
                case Subjects.Id.SecondaryModernForeignLanguages: coverage.SecondaryModernForeignLanguages = covered; break;
                case Subjects.Id.SecondaryScience: coverage.SecondaryScience = covered; break;
            }
        }
    }
    private async Task<IDictionary<string,LocalAuthorityDistrict>> GetInitialsToLocalAuthorityDistrictDictionary()
    {
        var localAuthorityDistrict = await _dbContext.LocalAuthorityDistricts
            .OrderBy(e => e.Name)
            .ToDictionaryAsync(e => e.Name.ToLower());
        return localAuthorityDistrict;
    }
    private class LocalRegionalTuitionPartnerDatum
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string Name { get; set; } = null!;
        [CsvHelper.Configuration.Attributes.Index(1)]
        public string? PrimaryLiteracyLocalRegionDistricts{ get; set; }
        [CsvHelper.Configuration.Attributes.Index(2)]
        public string? PrimaryNumeracyLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(3)]
        public string? PrimaryScienceLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(4)]
        public string? SecondaryMathsLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(5)]
        public string? SecondaryEnglishLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(6)]
        public string? SecondaryScienceLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(7)]
        public string? SecondaryModernForeignLanguagesLocalRegionDistricts { get; set; }
        [CsvHelper.Configuration.Attributes.Index(8)]
        public string? SecondaryHumanitiesLocalRegionDistricts { get; set; }
    }
}

