using Application.Extensions;
using Application.Extraction;
using Application.Factories;
using Domain;
using Domain.Constants;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Factories;

public class TribalSpreadsheetTuitionPartnerFactory : ITuitionPartnerFactoryStrategy
{
    public const string OrganisationDetailsSheetName = "Organisation Details";
    private const string PricingSheetName = "Pricing";
    private const string DeliverySheetName = "Delivery";
    private const int MaxRows = 100000;

    private readonly string _filename;
    private readonly ILogger _logger;
    private readonly NtpDbContext _dbContext;
    private readonly IDictionary<string, ImportMap> _organisationDetailsMapping;

    private ISpreadsheetExtractor? _spreadsheetExtractor;

    private readonly List<string> _warnings = new();
    private readonly List<string> _errors = new();

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

    public TribalSpreadsheetTuitionPartnerFactory(ILogger logger, string filename, NtpDbContext dbContext)
    {
        _logger = logger;
        _filename = filename;
        _dbContext = dbContext;

        TuitionPartner tpMapping = new();
        _organisationDetailsMapping = new Dictionary<string, ImportMap>
        {
            { "Organisation_Ref_ID_s", new ImportMap() },
            { "Organisation_s", new ImportMap(tpMapping, nameof(tpMapping.Name)) },
            { "Organisation_Address1_s", new ImportMap(tpMapping, nameof(tpMapping.Address)) },
            { "Organisation_Address2_s", new ImportMap() },
            { "Organisation_Town_s", new ImportMap() { IsRequired = true } },
            { "Organisation_County_s", new ImportMap() { IsRequired = true } },
            { "Organisation_PostCode_s", new ImportMap() { IsRequired = true } },
            { "Organisation_Tel_s", new ImportMap(tpMapping, nameof(tpMapping.PhoneNumber)) },
            { "Organisation_TP_Link_s", new ImportMap(tpMapping, nameof(tpMapping.Website)) },
            { "Organisation_Email_s", new ImportMap(tpMapping, nameof(tpMapping.Email)) },
            { "Organisation_Website_s", new ImportMap() },
            { "Organisation_ContactMethodPref_s", new ImportMap() },
            { "Organisation_Introduction_s", new ImportMap(tpMapping, nameof(tpMapping.AdditionalServiceOfferings)) },
            { "Organisation_LegalStatus_s", new ImportMap(tpMapping, nameof(tpMapping.LegalStatus)) },
            { "Organisation_LogoVector_s", new ImportMap() },
            { "Organisation_SENProvision_s", new ImportMap(tpMapping, nameof(tpMapping.HasSenProvision)) },
            { "Organisation_ChargeVAT_s", new ImportMap(tpMapping, nameof(tpMapping.IsVatCharged)) },
            { "Organisation_VATIncluded_s", new ImportMap() },
            { "Organisation_SEN_KS12_Support_s", new ImportMap() },
            { "Organisation_SEN_ASD_s", new ImportMap() },
            { "Organisation_SEN_HI_s", new ImportMap() },
            { "Organisation_SEN_MLD_s", new ImportMap() },
            { "Organisation_SEN_MSI_s", new ImportMap() },
            { "Organisation_SEN_PD_s", new ImportMap() },
            { "Organisation_SEN_PMLD_s", new ImportMap() },
            { "Organisation_SEN_SEMH_s", new ImportMap() },
            { "Organisation_SEN_SLNC_s", new ImportMap() },
            { "Organisation_SEN_SLD_s", new ImportMap() },
            { "Organisation_SEN_SPLD_s", new ImportMap() },
            { "Organisation_SEN_VI_S", new ImportMap() },
            { "Organisation_SEN_OTH_s", new ImportMap() },
            { "Organisation_Tutor_Description_s", new ImportMap(tpMapping, nameof(tpMapping.Description)) },
            { "Organisation_Tutor_CriteriaEmpTutor_s", new ImportMap() },
            { "Organisation_Tutor_Availability_s", new ImportMap() },
            { "Organisation_Tutor_TuitionSend_s", new ImportMap() },
            { "Organisation_Tutor_Processes_s", new ImportMap() },
            { "Organisation_Tutor_RecCriteria_s", new ImportMap() },
            { "Organisation_Tutor_Quals_s", new ImportMap() },
            { "Organisation_Tutor_IfNotQTS_s", new ImportMap() },
            { "Organisation_Tutor_LevelExp_s", new ImportMap(tpMapping, nameof(tpMapping.Experience)) },//TODO - check this map and all others once confirmed with Tribal
            { "Organisation_Tutor_LevelExp_Classroom_b", new ImportMap() },
            { "Organisation_Tutor_LevelExp_Teachers_b", new ImportMap() },
            { "Organisation_Tutor_LevelExp_Graduates_b", new ImportMap() },
            { "Organisation_Tutor_LevelExp_Undergraduates_b", new ImportMap() },
            { "Organisation_Tutor_LevelExp_Other_b", new ImportMap() },
            { "Organisation_Tutor_LevelExp_NoCriteria_b", new ImportMap() },
            { "Organisaiton_Exam_AQA_b", new ImportMap() },
            { "Organisation_Exam_OCR_b", new ImportMap() },
            { "Organisation_Exam_Pearson_b", new ImportMap() },
            { "Organisation_Exam_WJEC_b", new ImportMap() },
            { "Organisation_Exam_CAIE_b", new ImportMap() },
            { "Organisation_Exam_CCEA_b", new ImportMap() },
            { "Organisation_Exam_Other_b", new ImportMap() },
            { "Organisation_AddService_Service_s", new ImportMap() },
            { "Organisation_AddService_FreeService_s", new ImportMap() },
            { "Organisation_AddService_ChargeService_s", new ImportMap() },
            { "Organisation_Reference_s", new ImportMap() },
            { "Organisation_SEN_KS34_Support_s", new ImportMap() }
        };
    }

    public async Task<TuitionPartner> GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, CancellationToken cancellationToken)
    {
        TuitionPartner tuitionPartner = new();

        _spreadsheetExtractor = spreadsheetExtractor;

        if (!_spreadsheetExtractor.WorksheetExists(OrganisationDetailsSheetName))
        {
            _errors.Add($"Missing '{OrganisationDetailsSheetName}' worksheet");
        }
        if (!_spreadsheetExtractor.WorksheetExists(PricingSheetName))
        {
            _errors.Add($"Missing '{PricingSheetName}' worksheet");
        }
        if (!_spreadsheetExtractor.WorksheetExists(DeliverySheetName))
        {
            _errors.Add($"Missing '{DeliverySheetName}' worksheet");
        }

        if (_errors.Count == 0)
        {
            tuitionPartner = GetOrganisationDetails();

            tuitionPartner = await AddLocalAuthorityDistrictCoverage(tuitionPartner, cancellationToken);

            tuitionPartner = AddSubjectCoverageAndPrice(tuitionPartner);
        }

        //TODO - review how the multiple errors/warnings are logged in logit, ensure looks OK
        if (_warnings.Any())
        {
            _logger.LogWarning("Issues importing Tribal spreadsheet '{filename}': {warnings}", _filename, string.Join(Environment.NewLine, _warnings));
        }

        if (_errors.Any())
        {
            throw new Exception($"Error importing Tribal spreadsheet '{_filename}': {string.Join(Environment.NewLine, _errors)}");
        }

        return tuitionPartner;
    }

    private TuitionPartner GetOrganisationDetails()
    {
        const string KeyColumn = "A";
        const string ValueColumn = "C";
        const string KeyColumnTableHeader = "Title";
        TuitionPartner tuitionPartner = new();

        var completedLoop = false;
        var passedHeader = false;
        int row = 1;
        while (!completedLoop)
        {
            var key = _spreadsheetExtractor!.GetCellValue(OrganisationDetailsSheetName, KeyColumn, row);
            if (passedHeader)
            {
                if (row == MaxRows)
                {
                    _errors.Add($"Searched '{MaxRows}' rows in '{DeliverySheetName}' worksheet");
                    completedLoop = true;
                }
                else if (!string.IsNullOrWhiteSpace(key))
                {
                    var value = _spreadsheetExtractor.GetCellValue(OrganisationDetailsSheetName, ValueColumn, row);
                    if (_organisationDetailsMapping.ContainsKey(key))
                    {
                        if (!_organisationDetailsMapping[key].HasConvertedValue)
                        {
                            _organisationDetailsMapping[key].SetValue(value);
                        }
                        else
                        {
                            _errors.Add($"Duplicate '{key}' in '{DeliverySheetName}' worksheet");
                        }
                    }
                    else
                    {
                        _warnings.Add($"Unexpected '{key}' key exists in '{DeliverySheetName}' worksheet");
                    }
                }
                else
                {
                    completedLoop = true;
                }
            }
            else if (key == KeyColumnTableHeader)
            {
                passedHeader = true;
            }

            row++;
        }

        var missingNTPProperties = _organisationDetailsMapping.Where(x => !x.Value.IsInSourceData);
        if (missingNTPProperties.Any())
        {
            _warnings.Add($"The following were expected and not supplied in the '{DeliverySheetName}' worksheet: {string.Join(", ", missingNTPProperties.Select(x => x.Key).ToArray())}");
        }
        var missingRequiredNTPProperties = _organisationDetailsMapping.Where(x => x.Value.IsRequired && string.IsNullOrWhiteSpace(x.Value.SourceValue));
        if (missingRequiredNTPProperties.Any())
        {
            _errors.Add($"The following were required and not set in the '{DeliverySheetName}' worksheet: {string.Join(", ", missingRequiredNTPProperties.Select(x => x.Key).ToArray())}");
        }

        var mappedProperties = _organisationDetailsMapping.Where(x => x.Value.IsStoredInNtp && x.Value.HasConvertedValue);
        foreach (var mappedProperty in mappedProperties)
        {
            mappedProperty.Value.ApplyConvertedValueToProperty(tuitionPartner);
        }

        tuitionPartner.Website = tuitionPartner.Website.ParseUrl();
        //If no TP specific website then use the main website for the company
        if (string.IsNullOrWhiteSpace(tuitionPartner.Website))
        {
            tuitionPartner.Website = _organisationDetailsMapping.SingleOrDefault(x => x.Key == "Organisation_Website_s").Value.SourceValue.ParseUrl();
        }

        //Populate Address from multiple cells
        var addressLines = new string?[]
        {
        tuitionPartner.Address,
        _organisationDetailsMapping.SingleOrDefault(x => x.Key == "Organisation_Address2_s").Value.SourceValue,
        _organisationDetailsMapping.SingleOrDefault(x => x.Key == "Organisation_Town_s").Value.SourceValue,
        _organisationDetailsMapping.SingleOrDefault(x => x.Key == "Organisation_County_s").Value.SourceValue,
        _organisationDetailsMapping.SingleOrDefault(x => x.Key == "Organisation_PostCode_s").Value.SourceValue
        };

        tuitionPartner.Address = string.Join(Environment.NewLine, addressLines.Where(x => x != null));

        tuitionPartner.SeoUrl = tuitionPartner.Name.ToSeoUrl() ?? "";

        return tuitionPartner;
    }


    private async Task<TuitionPartner> AddLocalAuthorityDistrictCoverage(TuitionPartner tuitionPartner, CancellationToken cancellationToken)
    {
        var isInSchoolNationwide = _spreadsheetExtractor!.GetCellValue(DeliverySheetName, "B", 6).ParseBoolean();
        var isOnlineNationwide = _spreadsheetExtractor!.GetCellValue(DeliverySheetName, "C", 6).ParseBoolean();

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
            //TODO - stop the repeat db hits - do in memory, maybe even in a static list at class level for subsequent files
            var lad = await _dbContext.LocalAuthorityDistricts
                .FirstOrDefaultAsync(e => e.Code == ladCode, cancellationToken);

            //TODO - for tribal, do we want to validate the LAD name, Region code or region name as well, maybe as a warning?

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
        var regionCodes = _spreadsheetExtractor!.GetColumnValues(DeliverySheetName, "E", 6, 15);
        var inSchoolRegionsCovered = _spreadsheetExtractor.GetColumnValues(DeliverySheetName, "G", 6, 15);
        var onlineRegionsCovered = _spreadsheetExtractor.GetColumnValues(DeliverySheetName, "H", 6, 15);

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
        var ladCodes = _spreadsheetExtractor!.GetColumnValues(DeliverySheetName, "K", 6, 315);
        var inSchoolLadsCovered = _spreadsheetExtractor.GetColumnValues(DeliverySheetName, "M", 6, 315);
        var onlineLadsCovered = _spreadsheetExtractor.GetColumnValues(DeliverySheetName, "N", 6, 315);

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
        //TODO - if they have no LADs with online or none with in school then shouldn't include them here?  And have warning.
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