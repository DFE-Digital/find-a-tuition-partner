using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain;
using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Factories;

public class OpenXmlFactory
{
    private const string GeneralInformationSheetName = "General information";
    private const string PricingSheetName = "Pricing, Key Stage and SEN";
    private const string LocationSheetName = "Location of Tuition Provision";

    private static readonly IDictionary<(int, int), (string, int)> SubjectPricesCellReferences = new Dictionary<(int, int), (string, int)>
        {
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage1Literacy), ("C", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage1Numeracy), ("D", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage1Science), ("E", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage2Literacy), ("F", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage2Numeracy), ("G", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage2Science), ("H", 15) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage3English), ("C", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage3Humanities), ("D", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage3Maths), ("E", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage3Science), ("G", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage4English), ("H", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage4Humanities), ("I", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage4Maths), ("J", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage4ModernForeignLanguages), ("K", 25) },
            { (TuitionTypes.Id.InPerson, Subjects.Id.KeyStage4Science), ("K", 25) },

            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage1Literacy), ("C", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage1Numeracy), ("D", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage1Science), ("E", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage2Literacy), ("F", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage2Numeracy), ("G", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage2Science), ("H", 35) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage3English), ("C", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage3Humanities), ("D", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage3Maths), ("E", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage3ModernForeignLanguages), ("F", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage3Science), ("G", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage4English), ("H", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage4Humanities), ("I", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage4Maths), ("J", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage4ModernForeignLanguages), ("K", 46) },
            { (TuitionTypes.Id.Online, Subjects.Id.KeyStage4Science), ("K", 46) }
        };

    public static TuitionPartner? GetTuitionPartner(ILogger logger, Stream stream, NtpDbContext dbContext)
    {
        using var document = SpreadsheetDocument.Open(stream, false);

        var workbookPart = document.WorkbookPart;
        if (workbookPart == null)
        {
            logger.LogWarning("Spreadsheet workbook part was null");
            return null;
        }

        var tuitionPartner = new TuitionPartner
        {
            Name = GetCellValue(workbookPart, GeneralInformationSheetName, "C3"),
            Website = GetCellValue(workbookPart, GeneralInformationSheetName, "C4"),
            Email = GetCellValue(workbookPart, GeneralInformationSheetName, "D5"),
            PhoneNumber = GetCellValue(workbookPart, GeneralInformationSheetName, "D6"),
            Address = GetCellValue(workbookPart, GeneralInformationSheetName, "D8"),
            Description = GetCellValue(workbookPart, GeneralInformationSheetName, "C15")
        };

        var isInPersonNationwide = GetBooleanCellValue(workbookPart, LocationSheetName, "E24");
        var isOnlineNationwide = GetBooleanCellValue(workbookPart, LocationSheetName, "F24");

        var inPersonRegions = GetCellValue(workbookPart, LocationSheetName, "G24");
        var onlineRegions = GetCellValue(workbookPart, LocationSheetName, "H24");

        var inPersonLocalAuthorityDistricts = GetCellValue(workbookPart, LocationSheetName, "I24");
        var onlineLocalAuthorityDistricts = GetCellValue(workbookPart, LocationSheetName, "J24");

        var inPersonLads = GetLocalAuthorityDistricts(dbContext, isInPersonNationwide, inPersonRegions, inPersonLocalAuthorityDistricts);
        var onlineLads = GetLocalAuthorityDistricts(dbContext, isOnlineNationwide, onlineRegions, onlineLocalAuthorityDistricts);

        var supportedTuitionTypeSubjects = new Dictionary<int, HashSet<int>>
        {
            {TuitionTypes.Id.InPerson, new HashSet<int>()},
            {TuitionTypes.Id.Online, new HashSet<int>()}
        };

        foreach(var ((tuitionTypeId, subjectId), (column, row)) in SubjectPricesCellReferences)
        {
            var prices = new decimal[6];
            for (var i = 0; i < 6; i++)
            {
                var groupSize = i + 1;
                var addressName = $"{column}{row + i}";

                var cellPriceContent = GetCellValue(workbookPart, PricingSheetName, addressName);

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
                supportedTuitionTypeSubjects[tuitionTypeId].Add(subjectId);
            }
        }

        foreach (var localAuthorityDistrict in inPersonLads)
        {
            if (!supportedTuitionTypeSubjects.TryGetValue(TuitionTypes.Id.InPerson, out var supportedSubjects)) break;

            tuitionPartner.Coverage.Add(new TuitionPartnerCoverage
            {
                TuitionPartner = tuitionPartner,
                LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                LocalAuthorityDistrict = localAuthorityDistrict,
                TuitionTypeId = TuitionTypes.Id.InPerson,
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
            if (!supportedTuitionTypeSubjects.TryGetValue(TuitionTypes.Id.Online, out var supportedSubjects)) break;

            tuitionPartner.Coverage.Add(new TuitionPartnerCoverage
            {
                TuitionPartner = tuitionPartner,
                LocalAuthorityDistrictId = localAuthorityDistrict.Id,
                LocalAuthorityDistrict = localAuthorityDistrict,
                TuitionTypeId = TuitionTypes.Id.Online,
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

    private static ICollection<LocalAuthorityDistrict> GetLocalAuthorityDistricts(NtpDbContext dbContext, bool isNationwide, string regionsString, string localAuthorityDistrictsString)
    {
        if (isNationwide)
        {
            return dbContext.LocalAuthorityDistricts.OrderBy(e => e.Code).ToList();
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
                        return dbContext.LocalAuthorityDistricts
                            .Where(e => localAuthorityDistrictIdentifiers.Contains(e.Code))
                            .OrderBy(e => e.Code)
                            .ToList();
                    }

                    return dbContext.LocalAuthorityDistricts
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

                var regions = dbContext.Regions
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

    private static bool GetBooleanCellValue(WorkbookPart wbPart, string sheetName, string addressName)
    {
        var value = GetCellValue(wbPart, sheetName, addressName);
        return value.StartsWith("Y");
    }

    private static string GetCellValue(WorkbookPart wbPart, string sheetName, string addressName)
    {
        string value = "";

        // Find the sheet with the supplied name, and then use that 
        // Sheet object to retrieve a reference to the first worksheet.
        Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
            Where(s => s.Name == sheetName).FirstOrDefault();

        // Throw an exception if there is no sheet.
        if (theSheet == null)
        {
            throw new ArgumentException("sheetName");
        }

        // Retrieve a reference to the worksheet part.
        WorksheetPart wsPart =
            (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

        // Use its Worksheet property to get a reference to the cell 
        // whose address matches the address you supplied.
        Cell theCell = wsPart.Worksheet.Descendants<Cell>().
            Where(c => c.CellReference == addressName).FirstOrDefault();

        // If the cell does not exist, return an empty string.
        if (theCell != null)
        {
            value = theCell.InnerText;

            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that 
            // represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code 
            // looks up the corresponding value in the shared string 
            // table. For Booleans, the code converts the value into 
            // the words TRUE or FALSE.
            if (theCell.DataType != null)
            {
                switch (theCell.DataType.Value)
                {
                    case CellValues.SharedString:

                        // For shared strings, look up the value in the
                        // shared strings table.
                        var stringTable =
                            wbPart.GetPartsOfType<SharedStringTablePart>()
                            .FirstOrDefault();

                        // If the shared string table is missing, something 
                        // is wrong. Return the index that is in
                        // the cell. Otherwise, look up the correct text in 
                        // the table.
                        if (stringTable != null)
                        {
                            value =
                                stringTable.SharedStringTable
                                .ElementAt(int.Parse(value)).InnerText;
                        }
                        break;

                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
            }
        }

        return value;
    }
}