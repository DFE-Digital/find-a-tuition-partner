using Application.Extensions;
using Application.Extraction;
using Application.Factories;
using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Factories;

public class QualityAssuredSpreadsheetTuitionPartnerFactory : IQualityAssuredSpreadsheetTuitionPartnerFactory
{
    private readonly NtpDbContext _dbContext;

    private ISpreadsheetExtractor? _spreadsheetExtractor;

    private const string GeneralInformationSheetName = "General information";
    private const string PricingSheetName = "Pricing, Key Stage and SEN";
    private const string LocationSheetName = "Location of Tuition Provision";

    private static readonly IDictionary<(TuitionTypes, int), (string, int)> SubjectPricesCellReferences = new Dictionary<(TuitionTypes, int), (string, int)>
        {
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage1English), ("C", 7) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage1Maths), ("D", 7) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage1Science), ("E", 7) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage2English), ("C", 17) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage2Maths), ("D", 17) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage2Science), ("E", 17) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage3English), ("C", 27) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage3Humanities), ("D", 27) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage3Maths), ("E", 27) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 27) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage3Science), ("G", 27) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage4English), ("C", 37) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage4Humanities), ("D", 37) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage4Maths), ("E", 37) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage4ModernForeignLanguages), ("F", 37) },
            { (TuitionTypes.InSchool, Subjects.Id.KeyStage4Science), ("G", 37) },

            { (TuitionTypes.Online, Subjects.Id.KeyStage1English), ("C", 47) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage1Maths), ("D", 47) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage1Science), ("E", 47) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage2English), ("C", 57) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage2Maths), ("D", 57) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage2Science), ("E", 57) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage3English), ("C", 67) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage3Humanities), ("D", 67) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage3Maths), ("E", 67) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 67) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage3Science), ("G", 67) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage4English), ("C", 77) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage4Humanities), ("D", 77) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage4Maths), ("E", 77) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage4ModernForeignLanguages), ("F", 77) },
            { (TuitionTypes.Online, Subjects.Id.KeyStage4Science), ("G", 77) }
        };

    public QualityAssuredSpreadsheetTuitionPartnerFactory(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TuitionPartner> GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, CancellationToken cancellationToken)
    {
        _spreadsheetExtractor = spreadsheetExtractor;

        var tuitionPartner = new TuitionPartner
        {
            LastUpdated = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "F", 5).ParseDateOnly() ?? DateOnly.MinValue,
            Name = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 4),
            Website = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 5).ParseUrl(),
            Email = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 6),
            PhoneNumber = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 7),
            Address = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 9),
            Description = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "B", 13),
            Experience = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 13),
            LegalStatus = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "D", 13),
            HasSenProvision = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "F", 13).ParseBoolean(),
            AdditionalServiceOfferings = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "G", 13),
            IsVatCharged = _spreadsheetExtractor.GetCellValue(PricingSheetName, "K", 2).ParseBoolean(),
        };

        if (string.IsNullOrWhiteSpace(tuitionPartner.Website))
        {
            tuitionPartner.Website = _spreadsheetExtractor.GetCellValue(GeneralInformationSheetName, "C", 8).ParseUrl();
        }

        tuitionPartner.SeoUrl = tuitionPartner.Name.ToSeoUrl() ?? "";

        tuitionPartner = await AddLocalAuthorityDistrictCoverage(tuitionPartner, cancellationToken);

        tuitionPartner = AddSubjectCoverageAndPrice(tuitionPartner);

        return tuitionPartner;
    }

    private async Task<TuitionPartner> AddLocalAuthorityDistrictCoverage(TuitionPartner tuitionPartner, CancellationToken cancellationToken)
    {
        var isInSchoolNationwide = _spreadsheetExtractor!.GetCellValue(LocationSheetName, "B", 6).ParseBoolean();
        var isOnlineNationwide = _spreadsheetExtractor!.GetCellValue(LocationSheetName, "C", 6).ParseBoolean();

        var regionInitialsCovered = GetRegionInitialsCovered();

        var ladCodesCovered = GetLadCodesCovered();

        var ladsCovered = new Dictionary<(string code, TuitionTypes tuitionType), LocalAuthorityDistrict>();

        if (isInSchoolNationwide)
        {
            foreach (var lad in _dbContext.LocalAuthorityDistricts.OrderBy(e => e.Code))
            {
                ladsCovered[(lad.Code, TuitionTypes.InSchool)] = lad;
            }
        }

        if (isOnlineNationwide)
        {
            foreach (var lad in _dbContext.LocalAuthorityDistricts.OrderBy(e => e.Code))
            {
                ladsCovered[(lad.Code, TuitionTypes.Online)] = lad;
            }
        }

        foreach ((string regionInitials, (bool inSchoolCovered, bool onlineCovered)) in regionInitialsCovered)
        {
            if (!Regions.InitialsToId.TryGetValue(regionInitials, out var regionId))
            {
                throw new Exception($"Region initial {regionInitials} was not recognised");
            }

            var region = await _dbContext.Regions
                .Include(e => e.LocalAuthorityDistricts)
                .FirstOrDefaultAsync(e => e.Id == regionId, cancellationToken);

            if (region == null)
            {
                throw new Exception($"Region with id {regionId} from initial {regionInitials} was not found");
            }

            foreach (var lad in region.LocalAuthorityDistricts)
            {
                if (inSchoolCovered)
                {
                    ladsCovered[(lad.Code, TuitionTypes.InSchool)] = lad;
                }
                if (onlineCovered)
                {
                    ladsCovered[(lad.Code, TuitionTypes.Online)] = lad;
                }
            }
        }

        foreach ((string ladCode, (bool inSchoolCovered, bool onlineCovered)) in ladCodesCovered)
        {
            var lad = await _dbContext.LocalAuthorityDistricts
                .FirstOrDefaultAsync(e => e.Code == ladCode, cancellationToken);

            if (lad == null)
            {
                throw new Exception($"Local Authority District with code {ladCode} was not found");
            }

            if (inSchoolCovered)
            {
                ladsCovered[(lad.Code, TuitionTypes.InSchool)] = lad;
            }
            if (onlineCovered)
            {
                ladsCovered[(lad.Code, TuitionTypes.Online)] = lad;
            }
        }

        foreach (((_, TuitionTypes tuitionType), LocalAuthorityDistrict lad) in ladsCovered)
        {
            var coverage = new LocalAuthorityDistrictCoverage
            {
                TuitionPartner = tuitionPartner,
                TuitionTypeId = (int)tuitionType,
                LocalAuthorityDistrictId = lad.Id
            };

            tuitionPartner.LocalAuthorityDistrictCoverage.Add(coverage);
        }

        return tuitionPartner;
    }

    private IDictionary<string, (bool inSchoolCovered, bool onlineCovered)> GetRegionInitialsCovered()
    {
        var regionCodes = _spreadsheetExtractor!.GetColumnValues(LocationSheetName, "E", 6, 15);
        var inSchoolRegionsCovered = _spreadsheetExtractor.GetColumnValues(LocationSheetName, "G", 6, 15);
        var onlineRegionsCovered = _spreadsheetExtractor.GetColumnValues(LocationSheetName, "H", 6, 15);

        var regionCodesCovered = new Dictionary<string, (bool inSchoolCovered, bool onlineCovered)>();
        var index = 0;
        foreach (var regionCode in regionCodes)
        {
            var inSchoolRegionCovered = inSchoolRegionsCovered[index].ParseBoolean();
            var onlineRegionCovered = onlineRegionsCovered[index].ParseBoolean();
            if (inSchoolRegionCovered || onlineRegionCovered)
            {
                regionCodesCovered[regionCode] = (inSchoolRegionCovered, onlineRegionCovered);
            }

            index++;
        }

        return regionCodesCovered;
    }

    private IDictionary<string, (bool inSchoolCovered, bool onlineCovered)> GetLadCodesCovered()
    {
        var ladCodes = _spreadsheetExtractor!.GetColumnValues(LocationSheetName, "K", 6, 315);
        var inSchoolLadsCovered = _spreadsheetExtractor.GetColumnValues(LocationSheetName, "M", 6, 315);
        var onlineLadsCovered = _spreadsheetExtractor.GetColumnValues(LocationSheetName, "N", 6, 315);

        var ladCodesCovered = new Dictionary<string, (bool inSchoolCovered, bool onlineCovered)>();
        var index = 0;
        foreach (var ladCode in ladCodes)
        {
            var inSchoolLadCovered = inSchoolLadsCovered[index].ParseBoolean();
            var onlineLadCovered = onlineLadsCovered[index].ParseBoolean();
            if (inSchoolLadCovered || onlineLadCovered)
            {
                ladCodesCovered[ladCode] = (inSchoolLadCovered, onlineLadCovered);
            }

            index++;
        }

        return ladCodesCovered;
    }

    private TuitionPartner AddSubjectCoverageAndPrice(TuitionPartner tuitionPartner)
    {
        foreach (var ((tuitionType, subjectId), (column, row)) in SubjectPricesCellReferences)
        {
            var subjectHourlyRates = _spreadsheetExtractor!.GetColumnValues(PricingSheetName, column, row, row + 6)
                .Select(x => x.ParsePrice())
                .ToArray();

            var isSubjectSupported = subjectHourlyRates.Any(x => x > 0);
            if (isSubjectSupported)
            {
                var coverage = new SubjectCoverage
                {
                    TuitionPartner = tuitionPartner,
                    TuitionTypeId = (int)tuitionType,
                    SubjectId = subjectId
                };

                tuitionPartner.SubjectCoverage.Add(coverage);
            }

            for (int i = 0; i < subjectHourlyRates.Length; i++)
            {
                var subjectHourlyRate = subjectHourlyRates[i];

                if (subjectHourlyRate <= 0) continue;

                var price = new Price
                {
                    TuitionPartner = tuitionPartner,
                    TuitionTypeId = (int)tuitionType,
                    SubjectId = subjectId,
                    GroupSize = i + 1,
                    HourlyRate = subjectHourlyRate
                };

                tuitionPartner.Prices.Add(price);
            }
        }

        return tuitionPartner;
    }
}