using Domain;

namespace Application;

public interface ITuitionPartnerDataExtractor
{
    IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(FileInfo csvFile, int tuitionTypeId);
}