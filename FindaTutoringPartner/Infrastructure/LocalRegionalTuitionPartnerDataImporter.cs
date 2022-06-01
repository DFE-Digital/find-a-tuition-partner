using Application;
using Application.Extensions;
using Application.Repositories;
using Domain.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


namespace Infrastructure;

public class LocalRegionalTuitionPartnerDataImporter : ITuitionPartnerLocalRegionDataImporter
{

    private const string InPersonCsvFileName = "Infrastructure.Data.tuition_partners_face_to_face_local_regions.csv";
    private const string OnlineCsvFileName = "Infrastructure.Data.tuition_partners_online_local_regions.csv";

    private readonly ITuitionPartnerLocalRegionDataExtractor _extractor;
    private readonly NtpDbContext _dbContext;
    private readonly ITuitionPartnerRepository _repository;

    public LocalRegionalTuitionPartnerDataImporter(ITuitionPartnerLocalRegionDataExtractor extractor, NtpDbContext dbContext, ITuitionPartnerRepository repository)
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
        var md5Checksum = GetMd5Checksum(InPersonCsvFileName) + "_" + GetMd5Checksum(OnlineCsvFileName);
        var lastImport = await _dbContext.TuitionPartnerDataImportHistories.Where(e => e.Importer == GetType().Name).OrderByDescending(e => e.ImportDateTime).FirstOrDefaultAsync();
        if (lastImport?.Md5Checksum == md5Checksum)
        {
            return;
        }

        var inPerson = await _extractor.ExtractFromCsvFileAsync(InPersonCsvFileName, TuitionTypes.Id.InPerson).ToListAsync();
        var online = await _extractor.ExtractFromCsvFileAsync(OnlineCsvFileName, TuitionTypes.Id.Online).ToListAsync();


        var from = await _dbContext.TuitionPartners.AsNoTracking().Include(e => e.Coverage).OrderBy(e => e.Name).ToListAsync();
        var to = inPerson.Combine(online);

        var deltas = from.GetDeltas(to);

        await _repository.ApplyDeltas(deltas);

        _dbContext.TuitionPartnerDataImportHistories.Add(new TuitionPartnerDataImportHistory
        {
            Importer = GetType().Name,
            Md5Checksum = md5Checksum,
            ImportDateTime = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }
    private string GetMd5Checksum(string fileName)
    {
        using var md5 = MD5.Create();
        using var stream = typeof(AssemblyReference).Assembly.GetManifestResourceStream(fileName);
        var hash = md5.ComputeHash(stream ?? throw new InvalidOperationException());
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

