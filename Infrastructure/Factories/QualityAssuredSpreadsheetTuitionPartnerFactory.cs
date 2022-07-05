using Application.Extensions;
using Application.Extraction;
using Application.Factories;
using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Factories;

public class QualityAssuredSpreadsheetTuitionPartnerFactory : ITuitionPartnerFactory
{
    private readonly ILogger _logger;
    private readonly ISpreadsheetExtractor _spreadsheetExtractor;
    private readonly NtpDbContext _dbContext;

    private const string GeneralInformationSheetName = "General information";
    private const string PricingSheetName = "Pricing, Key Stage and SEN";
    private const string LocationSheetName = "Location of Tuition Provision";

    private static readonly IDictionary<(int, int), (string, int)> SubjectPricesCellReferences = new Dictionary<(int, int), (string, int)>
        {
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage1Literacy), ("C", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage1Numeracy), ("D", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage1Science), ("E", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage2Literacy), ("F", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage2Numeracy), ("G", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage2Science), ("H", 15) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage3English), ("C", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage3Humanities), ("D", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage3Maths), ("E", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage3Science), ("G", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage4English), ("H", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage4Humanities), ("I", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage4Maths), ("J", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage4ModernForeignLanguages), ("K", 25) },
            { ((int)TuitionTypes.InSchool, Subjects.Id.KeyStage4Science), ("K", 25) },

            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage1Literacy), ("C", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage1Numeracy), ("D", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage1Science), ("E", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage2Literacy), ("F", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage2Numeracy), ("G", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage2Science), ("H", 35) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage3English), ("C", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage3Humanities), ("D", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage3Maths), ("E", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage3Science), ("G", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage4English), ("H", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage4Humanities), ("I", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage4Maths), ("J", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage4ModernForeignLanguages), ("K", 46) },
            { ((int)TuitionTypes.Online, Subjects.Id.KeyStage4Science), ("K", 46) }
        };

    public QualityAssuredSpreadsheetTuitionPartnerFactory(ILogger<QualityAssuredSpreadsheetTuitionPartnerFactory> logger, ISpreadsheetExtractor spreadsheetExtractor, NtpDbContext dbContext)
    {
        _logger = logger;
        _spreadsheetExtractor = spreadsheetExtractor;
        _dbContext = dbContext;
    }

    public TuitionPartner GetTuitionPartner(Stream stream)
    {
        _spreadsheetExtractor.SetStream(stream);

        var tuitionPartner = new TuitionPartner
        {
            Name = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 4),
            Website = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 5),
            Email = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 6),
            PhoneNumber = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 7),
            Address = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 9),
            Description = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "B", 13),
            Experience = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 13),
            LegalStatus = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "D", 13),
            HasSenProvision = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "F", 13).ParseBoolean(),
            AdditionalServiceOfferings = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "G", 13)
        };

        var isInPersonNationwide = _spreadsheetExtractor.GetCellValue(LocationSheetName, "E", 24).ParseBoolean();
        var inPersonRegions = _spreadsheetExtractor.GetCellValue(LocationSheetName, "G", 24);
        var inPersonLocalAuthorityDistricts = _spreadsheetExtractor.GetCellValue(LocationSheetName, "I", 24);
        var inPersonLads = GetLocalAuthorityDistricts(isInPersonNationwide, inPersonRegions, inPersonLocalAuthorityDistricts);

        var isOnlineNationwide = _spreadsheetExtractor.GetCellValue(LocationSheetName, "F", 24).ParseBoolean();
        var onlineRegions = _spreadsheetExtractor.GetCellValue(LocationSheetName, "H", 24);
        var onlineLocalAuthorityDistricts = _spreadsheetExtractor.GetCellValue(LocationSheetName, "J", 24);
        var onlineLads = GetLocalAuthorityDistricts(isOnlineNationwide, onlineRegions, onlineLocalAuthorityDistricts);

        var supportedTuitionTypeLads = new Dictionary<TuitionTypes, ICollection<LocalAuthorityDistrict>>
        {
            {TuitionTypes.InSchool, inPersonLads},
            {TuitionTypes.Online, onlineLads}
        };

        foreach (var (tuitionTypeId, lads) in supportedTuitionTypeLads)
        {
            foreach (var lad in lads)
            {
                var coverage = new LocalAuthorityDistrictCoverage
                {
                    TuitionPartner = tuitionPartner,
                    TuitionTypeId = (int)tuitionTypeId,
                    LocalAuthorityDistrictId = lad.Id,
                };

                tuitionPartner.LocalAuthorityDistrictCoverage.Add(coverage);
            }
        }

        var supportedTuitionTypeSubjects = new Dictionary<int, HashSet<int>>
        {
            {(int)TuitionTypes.InSchool, new HashSet<int>()},
            {(int)TuitionTypes.Online, new HashSet<int>()}
        };

        foreach(var ((tuitionTypeId, subjectId), (column, row)) in SubjectPricesCellReferences)
        {
            var prices = new decimal[6];
            for (var i = 0; i < 6; i++)
            {
                var groupSize = i + 1;

                var cellPriceContent = _spreadsheetExtractor.GetCellValue(PricingSheetName, column, row + i);

                //TODO: Investigate 2 decimal point precision
                if (decimal.TryParse(cellPriceContent, out var hourlyRate))
                {
                    prices[i] = Math.Round(hourlyRate, 2);

                    var price = new Price
                    {
                        TuitionPartner = tuitionPartner,
                        TuitionTypeId = tuitionTypeId,
                        SubjectId = subjectId,
                        GroupSize = groupSize,
                        HourlyRate = hourlyRate
                    };

                    tuitionPartner.Prices.Add(price);
                }
            }

            var isSubjectSupported = prices.Any(x => x > 0);
            if (isSubjectSupported)
            {
                var coverage = new SubjectCoverage
                {
                    TuitionPartner = tuitionPartner,
                    TuitionTypeId = tuitionTypeId,
                    SubjectId = subjectId
                };

                tuitionPartner.SubjectCoverage.Add(coverage);

                supportedTuitionTypeSubjects[tuitionTypeId].Add(subjectId);
            }
        }

        foreach (var localAuthorityDistrict in inPersonLads)
        {
            if (!supportedTuitionTypeSubjects.TryGetValue((int)TuitionTypes.InSchool, out var supportedSubjects)) break;

            tuitionPartner.Coverage.Add(new TuitionPartnerCoverage
            {
                TuitionPartner = tuitionPartner,
                LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                LocalAuthorityDistrict = localAuthorityDistrict,
                TuitionTypeId = (int)TuitionTypes.InSchool,
                PrimaryLiteracy = supportedSubjects.Contains(Subjects.Id.KeyStage1Literacy) || supportedSubjects.Contains(Subjects.Id.KeyStage2Literacy),
                PrimaryNumeracy = supportedSubjects.Contains(Subjects.Id.KeyStage1Numeracy) || supportedSubjects.Contains(Subjects.Id.KeyStage2Numeracy),
                PrimaryScience = supportedSubjects.Contains(Subjects.Id.KeyStage1Science) || supportedSubjects.Contains(Subjects.Id.KeyStage2Science),
                SecondaryEnglish = supportedSubjects.Contains(Subjects.Id.KeyStage3English) || supportedSubjects.Contains(Subjects.Id.KeyStage4English),
                SecondaryHumanities = supportedSubjects.Contains(Subjects.Id.KeyStage3Humanities) || supportedSubjects.Contains(Subjects.Id.KeyStage4Humanities),
                SecondaryMaths = supportedSubjects.Contains(Subjects.Id.KeyStage3Maths) || supportedSubjects.Contains(Subjects.Id.KeyStage4Maths),
                SecondaryModernForeignLanguages = supportedSubjects.Contains(Subjects.Id.KeyStage3ModernForeignLanguages) || supportedSubjects.Contains(Subjects.Id.KeyStage4ModernForeignLanguages),
                SecondaryScience = supportedSubjects.Contains(Subjects.Id.KeyStage3Science) || supportedSubjects.Contains(Subjects.Id.KeyStage4Science)
            });
        }

        foreach (var localAuthorityDistrict in onlineLads)
        {
            if (!supportedTuitionTypeSubjects.TryGetValue((int)TuitionTypes.Online, out var supportedSubjects)) break;

            tuitionPartner.Coverage.Add(new TuitionPartnerCoverage
            {
                TuitionPartner = tuitionPartner,
                LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                LocalAuthorityDistrict = localAuthorityDistrict,
                TuitionTypeId = (int)TuitionTypes.Online,
                PrimaryLiteracy = supportedSubjects.Contains(Subjects.Id.KeyStage1Literacy) || supportedSubjects.Contains(Subjects.Id.KeyStage2Literacy),
                PrimaryNumeracy = supportedSubjects.Contains(Subjects.Id.KeyStage1Numeracy) || supportedSubjects.Contains(Subjects.Id.KeyStage2Numeracy),
                PrimaryScience = supportedSubjects.Contains(Subjects.Id.KeyStage1Science) || supportedSubjects.Contains(Subjects.Id.KeyStage2Science),
                SecondaryEnglish = supportedSubjects.Contains(Subjects.Id.KeyStage3English) || supportedSubjects.Contains(Subjects.Id.KeyStage4English),
                SecondaryHumanities = supportedSubjects.Contains(Subjects.Id.KeyStage3Humanities) || supportedSubjects.Contains(Subjects.Id.KeyStage4Humanities),
                SecondaryMaths = supportedSubjects.Contains(Subjects.Id.KeyStage3Maths) || supportedSubjects.Contains(Subjects.Id.KeyStage4Maths),
                SecondaryModernForeignLanguages = supportedSubjects.Contains(Subjects.Id.KeyStage3ModernForeignLanguages) || supportedSubjects.Contains(Subjects.Id.KeyStage4ModernForeignLanguages),
                SecondaryScience = supportedSubjects.Contains(Subjects.Id.KeyStage3Science) || supportedSubjects.Contains(Subjects.Id.KeyStage4Science)
            });
        }

        return tuitionPartner;
    }

    private ICollection<LocalAuthorityDistrict> GetLocalAuthorityDistricts(bool isNationwide, string regionsString, string localAuthorityDistrictsString)
    {
        if (isNationwide)
        {
            return _dbContext.LocalAuthorityDistricts.OrderBy(e => e.Code).ToList();
        }

        if (!string.IsNullOrWhiteSpace(localAuthorityDistrictsString))
        {
            var separator = GetSeparator(localAuthorityDistrictsString);

            if (!string.IsNullOrEmpty(separator))
            {
                var localAuthorityDistrictIdentifiers = localAuthorityDistrictsString.Split(separator);

                if (localAuthorityDistrictIdentifiers.Any())
                {
                    if (localAuthorityDistrictIdentifiers[0].StartsWith("E0"))
                    {
                        return _dbContext.LocalAuthorityDistricts
                            .Where(e => localAuthorityDistrictIdentifiers.Contains(e.Code))
                            .OrderBy(e => e.Code)
                            .ToList();
                    }

                    return _dbContext.LocalAuthorityDistricts
                        .Where(e => localAuthorityDistrictIdentifiers.Contains(e.Name))
                        .OrderBy(e => e.Code)
                        .ToList();

                    //TODO: Check we've found all the listed LADs
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(regionsString))
        {
            var separator = GetSeparator(regionsString);

            var regionInitials = regionsString.Split(separator);
            if (regionInitials.Any())
            {
                var regionIds = Regions.InitialsToId.Where(x => regionInitials.Contains(x.Key)).Select(x => x.Value);

                var regions = _dbContext.Regions
                    .Include(e => e.LocalAuthorityDistricts)
                    .Where(e => regionIds.Contains(e.Id))
                    .ToList();

                return regions.SelectMany(e => e.LocalAuthorityDistricts).ToList();
            }
        }

        return new List<LocalAuthorityDistrict>();
    }

    private static string GetSeparator(string value)
    {
        if (value.Contains(",")) return ",";
        if (value.Contains(";")) return ";";
        if (value.Contains("\r\n")) return "\r\n";
        if (value.Contains("\n")) return "\n";
        return "";
    }
}