using System.Globalization;
using Application;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Domain;

namespace Infrastructure;

public class RegionalTuitionPartnerDataExtractor : ITuitionPartnerDataExtractor
{
    public async IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(FileInfo csvFile)
    {
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
                Name = datum.Name
            };

            yield return tuitionPartner;
        }
    }

    private class RegionalTuitionPartnerDatum
    {
        [Index(0)]
        public string Name { get; set; } = null!;
        [Index(1)]
        public string? PrimaryLiteracyRegions { get; set; }
        [Index(2)]
        public string? PrimaryNumeracyRegions { get; set; }
        [Index(3)]
        public string? PrimaryScienceRegions { get; set; }
        [Index(4)]
        public string? SecondaryEnglishRegions { get; set; }
        [Index(5)]
        public string? SecondaryHumanitiesRegions { get; set; }
        [Index(6)]
        public string? SecondaryMathsRegions { get; set; }
        [Index(7)]
        public string? SecondaryModernForeignLanguagesRegions { get; set; }
        [Index(8)]
        public string? SecondaryScienceRegions { get; set; }
    }

    /*new Subject {Id = 1, Name = "Primary - Literacy"},
            new Subject {Id = 2, Name = "Primary - Numeracy"},
            new Subject {Id = 3, Name = "Primary - Science"},
            new Subject {Id = 4, Name = "Secondary - English"},
            new Subject {Id = 5, Name = "Secondary - Humanities"},
            new Subject {Id = 6, Name = "Secondary - Maths"},
            new Subject {Id = 7, Name = "Secondary - Modern Foreign Languages"},
            new Subject {Id = 8, Name = "Secondary - Science"}*/
}