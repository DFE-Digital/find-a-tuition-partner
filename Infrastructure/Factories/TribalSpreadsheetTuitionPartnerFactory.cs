using Application.Extensions;
using Application.Extraction;
using Application.Factories;
using Domain;
using Domain.Constants;
using Infrastructure.Mapping;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Factories;

public class TribalSpreadsheetTuitionPartnerFactory : ITribalSpreadsheetTuitionPartnerFactory
{
    public const string OrganisationDetailsSheetName = "Organisation Details";
    private const string DeliverySheetName = "Delivery";
    private const string PricingSheetName = "Pricing";
    private const int MaxRows = 100000;

    private readonly ILogger _logger;
    private readonly IDictionary<string, ImportMap> _organisationDetailsMapping;

    private IList<Region>? _regions;
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

    public TribalSpreadsheetTuitionPartnerFactory(ILogger<TribalSpreadsheetTuitionPartnerFactory> logger)
    {
        _logger = logger;

        TuitionPartner tpMapping = new();
        _organisationDetailsMapping = new Dictionary<string, ImportMap>
        {
            { "Organisation_Ref_ID_s", new ImportMap() },
            { "Organisation_s", new ImportMap(tpMapping, nameof(tpMapping.Name)) },
            { "Organisation_Address1_s", new ImportMap(tpMapping, nameof(tpMapping.Address)) },
            { "Organisation_Address2_s", new ImportMap() { IsStoredInNtp = true } },
            { "Organisation_Town_s", new ImportMap() { IsStoredInNtp = true, IsRequired = true } },
            { "Organisation_County_s", new ImportMap() { IsStoredInNtp = true, IsRequired = true } },
            { "Organisation_PostCode_s", new ImportMap() { IsStoredInNtp = true, IsRequired = true } },
            { "Organisation_Tel_s", new ImportMap(tpMapping, nameof(tpMapping.PhoneNumber)) },
            { "Organisation_TP_Link_s", new ImportMap(tpMapping, nameof(tpMapping.Website)) },
            { "Organisation_Email_s", new ImportMap(tpMapping, nameof(tpMapping.Email)) },
            { "Organisation_Website_s", new ImportMap() },
            { "Organisation_ContactMethodPref_s", new ImportMap() },
            { "Organisation_Introduction_s", new ImportMap(tpMapping, nameof(tpMapping.AdditionalServiceOfferings)) {IsRequired = false} },
            { "Organisation_LegalStatus_s", new ImportMap(tpMapping, nameof(tpMapping.LegalStatus)) },
            { "Organisation_LogoVector_s", new ImportMap() },
            { "Organisation_SENProvision_s", new ImportMap(tpMapping, nameof(tpMapping.HasSenProvision)) {IsRequired = false} },
            { "Organisation_ChargeVAT_s", new ImportMap(tpMapping, nameof(tpMapping.IsVatCharged)) {IsRequired = false} },
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
            { "Organisation_Tutor_LevelExp_s", new ImportMap(tpMapping, nameof(tpMapping.Experience)) },
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

    public TuitionPartner GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, string filename, IList<Region> regions)
    {
        _regions = regions;

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

            PopulateLocalAuthorityDistrictCoverage(tuitionPartner);

            tuitionPartner = AddSubjectCoverageAndPrice(tuitionPartner);
        }

        //TODO - review how the multiple errors/warnings are logged in logit, ensure looks OK
        if (_warnings.Any())
        {
            _logger.LogWarning("Issues importing Tribal spreadsheet '{filename}': {warnings}", filename, string.Join(Environment.NewLine, _warnings));
        }

        if (_errors.Any())
        {
            throw new Exception($"Error importing Tribal spreadsheet '{filename}': {string.Join(Environment.NewLine, _errors)}");
        }

        return tuitionPartner;
    }

    private void ProcessSheet(string sheetName, string tableHeaderColumn, string tableHeader, string dataColumn, Action<ISpreadsheetExtractor?, string, string, int> addRowToDictionary)
    {
        var completedLoop = false;
        var passedHeader = false;
        int row = 1;
        while (!completedLoop)
        {
            if (row == MaxRows)
            {
                _errors.Add($"Searched '{MaxRows}' rows in '{sheetName}' worksheet");
                completedLoop = true;
            }
            else if (passedHeader)
            {
                var data = _spreadsheetExtractor!.GetCellValue(sheetName, dataColumn, row);
                if (!string.IsNullOrWhiteSpace(data))
                {
                    addRowToDictionary(_spreadsheetExtractor, sheetName, data, row);
                }
                else
                {
                    completedLoop = true;
                }
            }
            else
            {
                passedHeader = _spreadsheetExtractor!.GetCellValue(sheetName, tableHeaderColumn, row) == tableHeader;
            }

            row++;
        }
    }

    private TuitionPartner GetOrganisationDetails()
    {
        const string KeyColumn = "A";
        const string ValueColumn = "C";
        const string TableHeaderColumn = "A";
        const string TableHeader = "Title";

        TuitionPartner tuitionPartner = new();
        var sheetName = OrganisationDetailsSheetName;

        //Add the data from the spreadsheet in to the _organisationDetailsMapping dictionary value class
        ProcessSheet(sheetName, TableHeaderColumn, TableHeader, KeyColumn,
            (spreadsheetExtractor, sheetName, data, row) =>
            {
                var key = data;
                if (_organisationDetailsMapping.ContainsKey(key))
                {
                    if (!_organisationDetailsMapping[key].HasConvertedValue)
                    {
                        var value = spreadsheetExtractor!.GetCellValue(sheetName, ValueColumn, row);
                        _organisationDetailsMapping[key].SetValue(value);
                    }
                    else
                    {
                        _errors.Add($"Duplicate '{key}' in '{sheetName}' worksheet");
                    }
                }
                else
                {
                    _warnings.Add($"Unexpected '{key}' key exists in '{sheetName}' worksheet");
                }
            });

        //Use the _organisationDetailsMapping dictionary to push the data in to the TuitionPartner class
        var mappedProperties = _organisationDetailsMapping.Where(x => x.Value.IsStoredInNtp && x.Value.HasConvertedValue);
        foreach (var mappedProperty in mappedProperties)
        {
            mappedProperty.Value.ApplyConvertedValueToProperty(tuitionPartner);
        }

        //Update the TuitionPartner class with specific mapping info
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

        //Deal with warnings and errors
        var missingNTPProperties = _organisationDetailsMapping.Where(x => !x.Value.IsInSourceData);
        if (missingNTPProperties.Any())
        {
            _warnings.Add($"The following were expected and not supplied in the '{sheetName}' worksheet: {string.Join(", ", missingNTPProperties.Select(x => x.Key).ToArray())}");
        }
        var missingRequiredNTPProperties = _organisationDetailsMapping.Where(x => x.Value.IsRequired && string.IsNullOrWhiteSpace(x.Value.SourceValue));
        if (missingRequiredNTPProperties.Any())
        {
            _errors.Add($"The following were required and not set in the '{sheetName}' worksheet: {string.Join(", ", missingRequiredNTPProperties.Select(x => x.Key).ToArray())}");
        }

        return tuitionPartner;
    }

    private void PopulateLocalAuthorityDistrictCoverage(TuitionPartner tuitionPartner)
    {
        const string LADCodeColumn = "A";
        const string LADNameColumn = "B";
        const string RegionCodeColumn = "C";
        const string RegionNameColumn = "D";
        const string InSchoolColumn = "E";
        const string OnlineColumn = "F";

        const string TableHeaderColumn = "A";
        const string TableHeader = "LAD Code";

        var sheetName = DeliverySheetName;
        var ladsCovered = new Dictionary<string, (int ladId, string ladName, string regionCode, string regionName, bool inSchool, bool online)>();
        var regionsAndLADs = _regions!
            .SelectMany(r => r.LocalAuthorityDistricts
                .Select(l => new { RegionName = r.Name, LocalAuthorityDistrictId = l.Id, LocalAuthorityDistrictCode = l.Code, LocalAuthorityDistrictName = l.Name })
            );

        //Add the data from the spreadsheet in to the _organisationDetailsMapping dictionary value class
        ProcessSheet(sheetName, TableHeaderColumn, TableHeader, LADCodeColumn,
            (spreadsheetExtractor, sheetName, data, row) =>
            {
                var ladCode = data;
                if (!ladsCovered.ContainsKey(ladCode))
                {
                    var ladName = spreadsheetExtractor!.GetCellValue(sheetName, LADNameColumn, row);
                    var regionCode = spreadsheetExtractor!.GetCellValue(sheetName, RegionCodeColumn, row);
                    var regionName = spreadsheetExtractor!.GetCellValue(sheetName, RegionNameColumn, row);

                    if (string.IsNullOrWhiteSpace(ladName) ||
                        string.IsNullOrWhiteSpace(regionCode) ||
                        string.IsNullOrWhiteSpace(regionName))
                    {
                        _warnings.Add($"Missing LAD or Region details on '{sheetName}' worksheet for LAD Code '{ladCode}'");
                    }

                    var ladId = regionsAndLADs.FirstOrDefault(x => x.LocalAuthorityDistrictCode == ladCode);

                    if (ladId == null)
                    {
                        _errors.Add($"Cannot find LAD Id in NTP database for LAD Code '{ladCode}' in '{sheetName}' worksheet");
                    }
                    else
                    {
                        var inSchool = spreadsheetExtractor!.GetCellValue(sheetName, InSchoolColumn, row).ParseBoolean();
                        var online = spreadsheetExtractor!.GetCellValue(sheetName, OnlineColumn, row).ParseBoolean();

                        ladsCovered[ladCode] = (ladId.LocalAuthorityDistrictId, ladName, regionCode, regionName, inSchool, online);
                    }
                }
                else
                {
                    _errors.Add($"Duplicate '{ladCode}' in '{sheetName}' worksheet");
                }
            });

        if (ladsCovered.Count == 0)
        {
            _errors.Add($"No entries added in the '{sheetName}' worksheet");
        }
        else
        {
            foreach ((_, (int ladId, string ladName, string regionCode, string regionName, bool inSchool, bool online)) in ladsCovered)
            {
                if (inSchool)
                {
                    var coverage = new LocalAuthorityDistrictCoverage
                    {
                        TuitionPartner = tuitionPartner,
                        TuitionTypeId = (int)TuitionTypes.InSchool,
                        LocalAuthorityDistrictId = ladId
                    };

                    tuitionPartner.LocalAuthorityDistrictCoverage.Add(coverage);
                }
                if (online)
                {
                    var coverage = new LocalAuthorityDistrictCoverage
                    {
                        TuitionPartner = tuitionPartner,
                        TuitionTypeId = (int)TuitionTypes.Online,
                        LocalAuthorityDistrictId = ladId
                    };

                    tuitionPartner.LocalAuthorityDistrictCoverage.Add(coverage);
                }
            }

            if (!tuitionPartner.LocalAuthorityDistrictCoverage.Any())
            {
                _errors.Add($"No entries set to True in the '{sheetName}' worksheet");
            }
            else
            {
                //Warn for invalid LAD Name, Region Code and Region Name
                var invalidLADNames = ladsCovered
                    .Select(tribalData => tribalData.Value.ladName)
                    .Distinct()
                    .Where(tribalData => !string.IsNullOrWhiteSpace(tribalData) && !regionsAndLADs.Any(db => db.LocalAuthorityDistrictName == tribalData));
                if (invalidLADNames.Any())
                {
                    _warnings.Add($"Invalid LAD Names on '{sheetName}' worksheet: {string.Join(", ", invalidLADNames)}");
                }

                var invalidRegionNames = ladsCovered
                    .Select(tribalData => tribalData.Value.regionName)
                    .Distinct()
                    .Where(tribalData => !string.IsNullOrWhiteSpace(tribalData) && !regionsAndLADs.Any(db => db.RegionName == tribalData));
                if (invalidRegionNames.Any())
                {
                    _warnings.Add($"Invalid Region Names on '{sheetName}' worksheet: {string.Join(", ", invalidRegionNames)}");
                }

                List<string> invalidRegionCodes = new();
                var regionCodes = ladsCovered
                    .Select(tribalData => tribalData.Value.regionCode)
                    .Distinct();
                foreach (var regionCode in regionCodes)
                {
                    if (!Regions.InitialsToId.TryGetValue(regionCode, out var regionId))
                    {
                        invalidRegionCodes.Add(regionCode);
                    }
                }
                if (invalidRegionCodes.Any())
                {
                    _warnings.Add($"Invalid Region Codes on '{sheetName}' worksheet: {string.Join(", ", invalidRegionCodes)}");
                }
            }
        }
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