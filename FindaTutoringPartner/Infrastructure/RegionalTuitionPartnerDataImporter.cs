using System.Security.Cryptography;
using Application;
using Application.Extensions;
using Application.Repositories;
using Infrastructure.Constants;
using Infrastructure.Entities;
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
        var inPersonFileInfo = new FileInfo(@"Data/tuition_partners_face_to_face_regions.csv");
        var onlineFileInfo = new FileInfo(@"Data/tuition_partners_online_regions.csv");

        var md5Checksum = GetMd5Checksum(inPersonFileInfo) + "_" + GetMd5Checksum(onlineFileInfo);

        var lastImport = await _dbContext.TuitionPartnerDataImportHistories.Where(e => e.Importer == GetType().Name).OrderByDescending(e => e.ImportDateTime).FirstOrDefaultAsync();
        if (lastImport?.Md5Checksum == md5Checksum)
        {
            return;
        }

        var inPerson = await _extractor.ExtractFromCsvFileAsync(inPersonFileInfo, TuitionTypes.Id.InPerson).ToListAsync();
        var online = await _extractor.ExtractFromCsvFileAsync(onlineFileInfo, TuitionTypes.Id.Online).ToListAsync();

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

    private string GetMd5Checksum(FileInfo fileInfo)
    {
        using var md5 = MD5.Create();
        using var stream = fileInfo.OpenRead();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}