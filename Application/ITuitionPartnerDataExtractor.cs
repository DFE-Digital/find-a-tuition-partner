using Domain;

namespace Application;

public interface ITuitionPartnerDataExtractor
{
    IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(string fileName, int tuitionTypeId);
}