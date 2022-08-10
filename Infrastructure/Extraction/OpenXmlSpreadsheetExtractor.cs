using Application.Extraction;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Infrastructure.Extraction;

public class OpenXmlSpreadsheetExtractor : ISpreadsheetExtractor, IDisposable
{
    private SpreadsheetDocument? _document;

    public void SetStream(Stream stream)
    {
        _document?.Dispose();

        _document = SpreadsheetDocument.Open(stream, false);

        var workbookPart = _document.WorkbookPart;
        if (workbookPart == null)
        {
            throw new ArgumentException("Spreadsheet workbook could not be accessed from the supplied stream", nameof(stream));
        }
    }

    public string GetCellValue(string sheetName, string column, int row)
    {
        var cellReference = GetCellReference(column, row);
        var descendants = GetWorksheet(sheetName).Descendants<Cell>();
        var cell = descendants.FirstOrDefault(c => c.CellReference == cellReference);

        if (cell == null) return string.Empty;

        if (cell.CellFormula != null && cell.CellValue != null) return cell.CellValue.InnerText;

        var value = cell.InnerText;
        if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(value, out var stringTableIndex))
        {
            return GetSharedStringTable().ElementAt(stringTableIndex).InnerText;
        }

        return value;
    }

    public string[] GetColumnValues(string sheetName, string column, int startRow, int endRow)
    {
        var count = endRow - startRow;
        var values = new string[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = GetCellValue(sheetName, column, startRow + i);
        }
        return values;
    }

    private WorkbookPart GetWorkbookPart()
    {
        if (_document == null)
        {
            throw new Exception("SetStream must be called before any get value method");
        }

        var workbookPart = _document.WorkbookPart;
        if (workbookPart == null)
        {
            throw new Exception("Spreadsheet workbook could not be accessed from document");
        }

        return workbookPart;
    }

    private Worksheet GetWorksheet(string sheetName)
    {
        var workbookPart = GetWorkbookPart();

        var sheet = GetWorkbookPart().Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
        if (sheet == null || sheet.Id == null)
        {
            throw new ArgumentException($"Could not access sheet {sheetName}", nameof(sheetName));
        }

        var worksheetPart = workbookPart.GetPartById(sheet.Id!) as WorksheetPart;
        if (worksheetPart == null)
        {
            throw new ArgumentException($"Could not access sheet {sheetName} using id {sheet.Id}", nameof(sheetName));
        }

        return worksheetPart.Worksheet;
    }

    private SharedStringTable GetSharedStringTable()
    {
        var workbookPart = GetWorkbookPart();

        var sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
        if (sharedStringTablePart == null)
        {
            throw new Exception("Spreadsheet shared string table part could not be accessed from document");
        }

        return sharedStringTablePart.SharedStringTable;
    }

    private static string GetCellReference(string column, int row)
    {
        return $"{column.ToUpper()}{row}";
    }

    public void Dispose()
    {
        _document?.Dispose();
    }
}