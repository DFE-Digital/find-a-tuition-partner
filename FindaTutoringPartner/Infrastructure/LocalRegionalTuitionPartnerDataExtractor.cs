using Application;
using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;

public class LocalRegionalTuitionPartnerDataExtractor : ITuitionPartnerLocalRegionDataExtractor
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

    public LocalRegionalTuitionPartnerDataExtractor(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(string fileName, int tuitionTypeId)
    {
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
        }

        yield return null;
    }

    private class LocalRegionalTuitionPartnerDatum
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
        public string? SecondaryMathsRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(5)]
        public string? SecondaryEnglishRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(6)]
        public string? SecondaryScienceRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(7)]
        public string? SecondaryModernForeignLanguagesRegionInitials { get; set; }
        [CsvHelper.Configuration.Attributes.Index(8)]
        public string? SecondaryHumanitiesRegionInitials { get; set; }
    }


}

