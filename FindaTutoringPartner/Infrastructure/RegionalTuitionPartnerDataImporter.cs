using Application;
using Application.Extensions;

namespace Infrastructure;

public class RegionalTuitionPartnerDataImporter : ITuitionPartnerDataImporter
{
    private readonly ITuitionPartnerDataExtractor _extractor;

    public RegionalTuitionPartnerDataImporter(ITuitionPartnerDataExtractor extractor)
    {
        _extractor = extractor;
    }

    public void Import()
    {
        ImportAsync().Wait();
    }

    public async Task ImportAsync()
    {
        var inPerson = await _extractor.ExtractFromCsvFileAsync(new FileInfo(@"Data/tuition_partners_face_to_face_regions.csv")).ToListAsync();
        var online = await _extractor.ExtractFromCsvFileAsync(new FileInfo(@"Data/tuition_partners_online_regions.csv")).ToListAsync();

        var data = inPerson.Combine(online);
    }
}