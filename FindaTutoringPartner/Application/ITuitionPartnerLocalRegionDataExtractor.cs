using Domain;

namespace Application
{
    public  interface ITuitionPartnerLocalRegionDataExtractor
    {
        IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(string fileName, int tuitionTypeId);
    }
}
