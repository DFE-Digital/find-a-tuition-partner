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
    private const string LocationSheetName = "Location of Tuition Provision";

    public static TuitionPartner? GetTuitionPartner(ILogger logger, FileStream fileStream, NtpDbContext dbContext)
    {
        using var document = SpreadsheetDocument.Open(fileStream, false);

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

        //tuitionPartner.Coverage = TuitionPartnerCoverageFactory.GetCoverage(isInPersonNationwide, isOnlineNationwide, inPersonRegions, onlineRegions, inPersonLocalAuthorityDistricts, onlineLocalAuthorityDistricts);

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

            if (!string.IsNullOrWhiteSpace(separator))
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
        return value != null && value.StartsWith("Y");
    }

    private static string? GetCellValue(WorkbookPart wbPart, string sheetName, string addressName)
    {
        string value = null;

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