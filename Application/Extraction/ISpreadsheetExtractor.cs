namespace Application.Extraction;

public interface ISpreadsheetExtractor
{
    void SetStream(Stream stream);
    string GetCellValue(string sheetName, string column, int row);
    string[] GetColumnValues(string sheetName, string column, int startRow, string endRow);
}