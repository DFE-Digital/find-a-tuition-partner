namespace Application.Extraction;

public interface ISpreadsheetExtractor
{
    void SetStream(Stream stream);
    void PreloadSheet(string sheetName);
    string GetCellValue(string sheetName, string column, int row);
    string[] GetColumnValues(string sheetName, string column, int startRow, int endRow);
    bool WorksheetExists(string sheetName);
}