using Domain;

public  interface ITuitionPartnerLocalRegionDataExtractor
{
   IAsyncEnumerable<TuitionPartner> ExtractFromCsvFileAsync(string fileName, int tuitionTypeId);
}

