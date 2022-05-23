using Application;
using Application.Extensions;
using Application.Repositories;
using Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class RegionalTuitionPartnerDataImporter : ITuitionPartnerDataImporter
{
    private readonly ITuitionPartnerDataExtractor _extractor;
    private readonly NtpDbContext _dbContext;
    private readonly ITuitionPartnerRepository _repository;

    public RegionalTuitionPartnerDataImporter(ITuitionPartnerDataExtractor extractor, NtpDbContext dbContext, ITuitionPartnerRepository repository)
    {
        _extractor = extractor;
        _dbContext = dbContext;
        _repository = repository;
    }

    public void Import()
    {
        ImportAsync().Wait();
    }

    public async Task ImportAsync()
    {
        var inPerson = await _extractor.ExtractFromCsvFileAsync(new FileInfo(@"Data/tuition_partners_face_to_face_regions.csv"), TuitionTypes.Id.InPerson).ToListAsync();
        var online = await _extractor.ExtractFromCsvFileAsync(new FileInfo(@"Data/tuition_partners_online_regions.csv"), TuitionTypes.Id.Online).ToListAsync();

        var from = await _dbContext.TuitionPartners.Include(e => e.Coverage).OrderBy(e => e.Name).ToListAsync();
        var to = inPerson.Combine(online);

        var deltas = from.GetDeltas(to);

        await _repository.ApplyDeltas(deltas);
    }
}