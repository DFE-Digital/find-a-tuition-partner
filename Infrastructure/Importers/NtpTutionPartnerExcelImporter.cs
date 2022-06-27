﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Importers
{
    public class NtpTutionPartnerExcelImporter
    {
        public static void Import(string fileName, NtpDbContext dbContext, ILogger logger)
        {
            if (File.Exists(fileName))
            {
                // Retrieve the value in cell A1.
                string name = GetCellValue(fileName, "General information", "C3");

                if (!String.IsNullOrEmpty(name))
                {
                    string[] Names = name.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    if (Names.Length > 1)
                    {
                        name = Names[0];
                    }

                    Console.WriteLine(name);
                }
                
                // Retrieve the value in cell A1.
                string address = GetCellValue(fileName, "General information", "C4");
                Console.WriteLine(address);

                // Retrieve the value in cell A1.
                string description = GetCellValue(fileName, "General information", "C15");
                Console.WriteLine(description);

                Console.WriteLine("**********************************");

                var test = dbContext.TuitionPartners.FirstOrDefault(e => e.Name == name);

                if(test != null)
                {
                    Console.WriteLine($"Found partner id {test.Id} for {test.Name}");
                }

                var tuitionPartner = new TuitionPartner
                {
                    Name = name,
                    Website = address
                };
            }
        }
    
        private static string GetCellValue(string fileName,
                                          string sheetName,
                                          string addressName)
        {
            string value = null;

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

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
            }
            return value;
        }
    }
}
