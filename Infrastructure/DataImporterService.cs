using System.ComponentModel.DataAnnotations;
using System.Text;
using Application;
using Application.Common.Interfaces;
using Application.Constants;
using Application.DataImport;
using Application.Extensions;
using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Validators;
using Infrastructure.Mapping.Configuration;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private const double PercentageFailedGIASRecordsLogsError = 2;
    private const string NoChangesLabel = "No Changes";

    private readonly IHostApplicationLifetime _host;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private SortedDictionary<string, List<TuitionPartner>> _processedTPs = new();

    private ILocationFilterService? _locationService;

    public DataImporterService(IHostApplicationLifetime host, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string originalFilename = string.Empty;
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var dataFileEnumerable = scope.ServiceProvider.GetRequiredService<IDataFileEnumerable>();
        var logoFileEnumerable = scope.ServiceProvider.GetRequiredService<ILogoFileEnumerable>();
        var factory = scope.ServiceProvider.GetRequiredService<ITribalSpreadsheetTuitionPartnerFactory>();
        _locationService = scope.ServiceProvider.GetRequiredService<ILocationFilterService>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        _processedTPs = new SortedDictionary<string, List<TuitionPartner>>();

        try
        {

            await strategy.ExecuteAsync(
                async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                    originalFilename = await ImportTuitionPartnerFiles(dbContext, dataFileEnumerable, factory, cancellationToken);

                    await ImportTuitionPartnerLogos(dbContext, logoFileEnumerable, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                });

            LogProcessedTPsMessage(originalFilename);
        }
        finally
        {
            var generalInformatioAboutSchoolsRecords = scope.ServiceProvider.GetRequiredService<IGeneralInformationAboutSchoolsRecords>();
            var giasFactory = scope.ServiceProvider.GetRequiredService<ISchoolsFactory>();

            await strategy.ExecuteAsync(
                async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                    await ImportGeneralInformationAboutSchools(dbContext, generalInformatioAboutSchoolsRecords, giasFactory, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                });
        }

        _host.StopApplication();
    }

    private async Task ImportGeneralInformationAboutSchools(NtpDbContext dbContext, IGeneralInformationAboutSchoolsRecords generalInformatioAboutSchoolsRecords, ISchoolsFactory giasFactory, CancellationToken cancellationToken)
    {
        var localAuthorityDistrictsIds = dbContext.LocalAuthorityDistricts.Select(t => new { t.Code, t.Id })
            .ToDictionary(t => t.Code, t => t.Id);

        var localAuthorityIds = dbContext.LocalAuthority.Select(t => new { t.Id, t.Code, })
           .ToDictionary(t => t.Id, t => t.Code);

        _logger.LogInformation("Retrieving GIAS dataset");
        var result = generalInformatioAboutSchoolsRecords.GetSchoolDataAsync(cancellationToken);
        _logger.LogInformation("Retrieved {SchoolsCount} schools from GIAS dataset", result.Result.Count);

        var imported = 0;
        var failedValidation = 0;
        var importedSchoolUrns = new List<int>();

        foreach (SchoolDatum schoolDatum in result.Result.Where(s => s.IsValidForService()))
        {
            var isNewSchool = false;
            var establishmentName = schoolDatum.Name;

            var school = dbContext.Schools.FirstOrDefault(x => x.Urn == schoolDatum.Urn);
            if (school == null)
            {
                school = new School();
                isNewSchool = true;
            }

            await UpdateInvalidLaAndLad(schoolDatum, localAuthorityDistrictsIds, localAuthorityIds);

            try
            {
                school = giasFactory.GetSchool(school, schoolDatum, localAuthorityDistrictsIds, localAuthorityIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when creating General Information About Schools from record {OriginalFilename}", establishmentName);
                continue;
            }

            var validator = new SchoolValidator();
            var results = await validator.ValidateAsync(school, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogInformation($"OPEN Establishment name {{TuitionPartnerName}} {{Urn}} {{EstablishmentStatus}} {{EstablishmentTypeGroupId}} {{EstablishmentType}} General Information About Schools created from record {{originalFilename}} is not valid.{Environment.NewLine}{{Errors}}",
                        school.EstablishmentName, school.Urn, school.EstablishmentStatusId, school.EstablishmentTypeGroupId, schoolDatum.EstablishmentType, school.EstablishmentName, string.Join(Environment.NewLine, results.Errors));
                failedValidation++;
                continue;
            }

            if (isNewSchool)
            {
                dbContext.Schools.Add(school);
                _logger.LogInformation($"ADDED new establishment, name: {{TuitionPartnerName}}, URN: {{Urn}}",
                    school.EstablishmentName, school.Urn);
            }

            importedSchoolUrns.Add(school.Urn);
            imported++;
            if ((imported % 100) == 0)
            {
                _logger.LogInformation("Imported {Count} schools from GIAS dataset", imported);
            }
        }

        ReportGIASProcessingError(imported, failedValidation);

        DeactivateSchools(dbContext, importedSchoolUrns);

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully imported {Count} valid schools from GIAS dataset. {FailedValidationCount} schools failed validation", imported, failedValidation);
    }

    private async Task UpdateInvalidLaAndLad(SchoolDatum schoolDatum,
        Dictionary<string, int> localAuthorityDistrictsIds,
        Dictionary<int, string> localAuthorityIds)
    {
        if ((!localAuthorityDistrictsIds.Any(x => x.Key == schoolDatum.LocalAuthorityDistrictCode) ||
            !localAuthorityIds.Any(x => x.Key == schoolDatum.LocalAuthorityCode)) &&
            !string.IsNullOrWhiteSpace(schoolDatum.Postcode))
        {
            var locationData = await _locationService!.GetLocationFilterParametersAsync(schoolDatum.Postcode);
            if (locationData != null)
            {
                _logger.LogInformation($"GIAS data LA/LAD data invalid but found via postcode lookup. Establishment name {{TuitionPartnerName}} {{Urn}} with LA Code {{LocalAuthorityCode}} and LAD Code {{LocalAuthorityDistrictCode}}.  Postcode lookup found LA Code {{FoundLocalAuthorityCode}} and LAD Code {{FoundLocalAuthorityDistrictCode}}",
                    schoolDatum.Name, schoolDatum.Urn, schoolDatum.LocalAuthorityCode, schoolDatum.LocalAuthorityDistrictCode, locationData.LocalAuthorityId, locationData.LocalAuthorityDistrictCode);

                schoolDatum.LocalAuthorityDistrictCode = locationData.LocalAuthorityDistrictCode ?? schoolDatum.LocalAuthorityDistrictCode;
                schoolDatum.LocalAuthorityCode = locationData.LocalAuthorityId ?? schoolDatum.LocalAuthorityCode;
            }
        }
    }

    private void ReportGIASProcessingError(int imported, int failedValidation)
    {
        if (imported == 0)
        {
            throw new InvalidDataException("0 Schools imported from GIAS data");
        }

        if (PercentageFailedGIASRecordsLogsError > 0)
        {
            var totalProcessedRecords = imported + failedValidation;
            var failedPercentage = failedValidation / (double)totalProcessedRecords * 100;
            if (failedPercentage > PercentageFailedGIASRecordsLogsError)
            {
                _logger.LogError($"Too many failed records when processing the GIAS records, requires further investigation.  {{imported}} were imported successfully and {{failedValidation}} failed validation.  {{failedPercentage}}% failed, which is more than the {{PercentageFailedGIASRecordsLogsError}}% threshold.", imported, failedValidation, failedPercentage, PercentageFailedGIASRecordsLogsError);
            }
        }
    }

    private void DeactivateSchools(NtpDbContext dbContext, List<int> importedSchoolUrns)
    {
        var schoolsToDeactivate = dbContext.Schools.Where(x => !importedSchoolUrns.Contains(x.Urn) && x.IsActive).ToList();
        if (schoolsToDeactivate != null && schoolsToDeactivate.Any())
        {
            foreach (var schoolToDeactivate in schoolsToDeactivate)
            {
                schoolToDeactivate.IsActive = false;

                _logger.LogInformation($"DEACTIVATED establishment, name: {{TuitionPartnerName}}, URN: {{Urn}}",
                    schoolToDeactivate.EstablishmentName, schoolToDeactivate.Urn);
            }
            _logger.LogInformation("Deactivated {Count} schools from GIAS dataset", schoolsToDeactivate.Count);
        }
    }

    private async Task<string> ImportTuitionPartnerFiles(NtpDbContext dbContext, IDataFileEnumerable dataFileEnumerable, ITribalSpreadsheetTuitionPartnerFactory factory,
    CancellationToken cancellationToken)
    {
        var dataFile = GetImportTuitionPartnerFile(dataFileEnumerable);

        var regions = await dbContext.Regions
            .Include(e => e.LocalAuthorityDistricts)
            .ToListAsync(cancellationToken);

        var subjects = await dbContext.Subjects
            .ToListAsync(cancellationToken);

        var organisationTypes = await dbContext.OrganisationType
            .ToListAsync(cancellationToken);

        var allExistingTPs = await dbContext
                .TuitionPartners
                .Include(x => x.Prices)
                .Include(x => x.LocalAuthorityDistrictCoverage)
                .Include(x => x.SubjectCoverage)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

        var activeTps = allExistingTPs.Where(x => x.IsActive).Count();

        _logger.LogInformation("Number of existing active TPs {activeTps}, with {deactivatedTps} currently deactivated", activeTps, (allExistingTPs.Count - activeTps));

        var tpImportedDates = allExistingTPs
                .Select(x => new { x.Name, x.TPLastUpdatedData })
                .ToDictionary(x => x.Name.ToLower(), x => x.TPLastUpdatedData);

        DataImporterMappingConfig.Configure();

        var originalFilename = dataFile.Filename.ExtractFileNameFromDirectory();

        //Use Polly to retry up to 5 times per file read (in case of network issues)
        int numberOfRetries = 5;
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(numberOfRetries, retryAttempt =>
                //Wait 2, 4, 8, 16 and then 32 seconds
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, sleepDuration, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "{TPDataImportLogMessagePrefix}Exception thrown when reading Tuition Partner from file {OriginalFilename}.  Retrying in {SleepDuration}. Attempt {RetryCount} out of {NumberOfRetries}",
                        StringConstants.TPDataImportLogMessagePrefix, originalFilename, sleepDuration, retryCount, numberOfRetries);
                });


        _logger.LogInformation("Attempting to process Tuition Partner from file {OriginalFilename}", originalFilename);
        List<TuitionPartner> tuitionPartnersToProcess = new();
        try
        {
            tuitionPartnersToProcess = retryPolicy.Execute(() =>
            {
                //Uncomment to test polly
                //Random random = new();
                //if (random.Next(1, 3) == 1)
                //    throw new Exception("Testing Polly");

                return factory.GetTuitionPartners(dataFile.Stream.Value, originalFilename, regions, subjects, organisationTypes, tpImportedDates);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{TPDataImportLogMessagePrefix}Exception thrown when processing Tuition Partners from file {OriginalFilename}",
                StringConstants.TPDataImportLogMessagePrefix, originalFilename);
            //Errors reading the file then throw exception, so cancels the transaction and rollsback db.
            throw;
        }

        foreach (var tuitionPartnerToProcess in tuitionPartnersToProcess)
        {
            var validator = new TuitionPartnerValidator(GetAllProcessedTPs());
            var results = await validator.ValidateAsync(tuitionPartnerToProcess, cancellationToken);

            //There is a lot of validation within GetTuitionPartners, so it's unlikely that this will throw an error, but is here for completeness
            if (!results.IsValid)
            {
                var errorMsg = $"Tuition Partner name {tuitionPartnerToProcess.Name} created from file {originalFilename} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}";
                _logger.LogError(message: $"{StringConstants.TPDataImportLogMessagePrefix}{errorMsg}");
                //Errors validating any file then throw exception, so cancels the transaction and rollsback db.
                throw new ValidationException(errorMsg);
            }

            //Find existing TP in db
            var matchedTPs = allExistingTPs.Where(x => x.ImportId.Equals(tuitionPartnerToProcess.ImportId, StringComparison.InvariantCultureIgnoreCase) ||
                                                        x.SeoUrl == tuitionPartnerToProcess.SeoUrl).ToList();

            //If no match then add
            if (matchedTPs == null || matchedTPs.Count == 0)
            {
                SetImportingData(tuitionPartnerToProcess);

                dbContext.TuitionPartners.Add(tuitionPartnerToProcess);

                await dbContext.SaveChangesAsync(cancellationToken);

                AddToProcessedTPs("Added", tuitionPartnerToProcess);
            }
            else if (matchedTPs.Count == 1)
            {
                //If matched then see if needs updating

                var existingTP = matchedTPs.First();

                //Map spreadsheet data to existing entity
                existingTP = tuitionPartnerToProcess!.Adapt(existingTP);

                ImportTuitionPartnerLocalAuthorityDistrictCoverage(dbContext, existingTP, tuitionPartnerToProcess);
                ImportTuitionPartnerSubjectCoverage(dbContext, existingTP, tuitionPartnerToProcess);
                ImportTuitionPartnerPrices(dbContext, existingTP, tuitionPartnerToProcess);

                if (!existingTP.IsActive ||
                    dbContext.ChangeTracker.Entries().Any(e => e.State == EntityState.Modified ||
                                                                e.State == EntityState.Added ||
                                                                e.State == EntityState.Deleted))
                {
                    SetImportingData(existingTP);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    AddToProcessedTPs("Updated", tuitionPartnerToProcess);
                }
                else
                {
                    AddToProcessedTPs(NoChangesLabel, tuitionPartnerToProcess);
                }
            }
            else if (matchedTPs.Count > 1)
            {
                var errorMsg = $"The file {originalFilename} with seo url('{tuitionPartnerToProcess.SeoUrl}') and import id('{tuitionPartnerToProcess.ImportId}') is returning more than 1 result from the database.";
                _logger.LogError(message: $"{StringConstants.TPDataImportLogMessagePrefix}{errorMsg}");
                throw new InvalidOperationException(errorMsg);
            }
        }

        await DeactivateTPs(dbContext, allExistingTPs, cancellationToken);

        return originalFilename;
    }

    private void LogProcessedTPsMessage(string filename)
    {
        var sb = new StringBuilder($"The {filename} was successfully processed, with the following Tuition Partner outcomes.");
        var hasChanges = false;

        foreach (var kvp in _processedTPs)
        {
            if (kvp.Key != NoChangesLabel)
                hasChanges = true;

            sb.Append($"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{kvp.Key}:{Environment.NewLine}{Environment.NewLine}");

            sb.Append(string.Join(Environment.NewLine, kvp.Value.Select(x => $"{x.Name}")));
        }

        if (hasChanges)
            sb.Insert(0, StringConstants.TPDataImportLogMessagePrefix);

        _logger.LogInformation(sb.ToString());
    }

    private void AddToProcessedTPs(string key, TuitionPartner tuitionPartner)
    {
        if (_processedTPs.ContainsKey(key))
        {
            _processedTPs[key].Add(tuitionPartner);
        }
        else
        {
            _processedTPs.Add(key, new List<TuitionPartner>() { tuitionPartner });
        }
    }

    private List<TuitionPartner> GetAllProcessedTPs()
    {
        var tps = new List<TuitionPartner>();

        foreach (var kvp in _processedTPs)
        {
            tps.AddRange(kvp.Value);
        }

        return tps;
    }

    private static DataFile GetImportTuitionPartnerFile(IDataFileEnumerable dataFileEnumerable)
    {

        var dataFile = dataFileEnumerable.OrderByDescending(x => x.Filename).FirstOrDefault();

        return dataFile == null ? throw new InvalidDataException($"{StringConstants.TPDataImportLogMessagePrefix}No TP spreadsheets to import") : dataFile;
    }

    private static void ImportTuitionPartnerLocalAuthorityDistrictCoverage(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var laDistrictCoveragesToDelete = existingTP.LocalAuthorityDistrictCoverage.Where(x => !tuitionPartnerToProcess.LocalAuthorityDistrictCoverage.Any(e => e.TuitionSettingId == x.TuitionSettingId &&
                                                                                                                                                e.LocalAuthorityDistrictId == x.LocalAuthorityDistrictId));
        foreach (var laDistrictCoverageToDelete in laDistrictCoveragesToDelete)
        {
            dbContext.Entry(laDistrictCoverageToDelete).State = EntityState.Deleted;
        }

        foreach (var laDistrictCoverage in tuitionPartnerToProcess.LocalAuthorityDistrictCoverage)
        {
            var existingLaDistrictCoverage = existingTP.LocalAuthorityDistrictCoverage.FirstOrDefault(x => x.TuitionSettingId == laDistrictCoverage.TuitionSettingId &&
                                                                                                        x.LocalAuthorityDistrictId == laDistrictCoverage.LocalAuthorityDistrictId);

            if (existingLaDistrictCoverage == null)
            {
                existingTP.LocalAuthorityDistrictCoverage.Add(laDistrictCoverage);
            }
            else
            {
                //Update the existing data, will be updated since ref to object
                existingLaDistrictCoverage = laDistrictCoverage.Adapt(existingLaDistrictCoverage);
            }
        }
    }

    private static void ImportTuitionPartnerSubjectCoverage(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var subjectCoveragesToDelete = existingTP.SubjectCoverage.Where(x => !tuitionPartnerToProcess.SubjectCoverage.Any(e => e.TuitionSettingId == x.TuitionSettingId &&
                                                                                                                            e.SubjectId == x.SubjectId));
        foreach (var subjectCoverageToDelete in subjectCoveragesToDelete)
        {
            dbContext.Entry(subjectCoverageToDelete).State = EntityState.Deleted;
        }

        foreach (var subjectCoverage in tuitionPartnerToProcess.SubjectCoverage)
        {
            var existingSubjectCoverage = existingTP.SubjectCoverage.FirstOrDefault(x => x.TuitionSettingId == subjectCoverage.TuitionSettingId &&
                                                                                x.SubjectId == subjectCoverage.SubjectId);

            if (existingSubjectCoverage == null)
            {
                existingTP.SubjectCoverage.Add(subjectCoverage);
            }
            else
            {
                //Update the existing data, will be updated since ref to object
                existingSubjectCoverage = subjectCoverage.Adapt(existingSubjectCoverage);
            }
        }
    }

    private static void ImportTuitionPartnerPrices(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var pricesToDelete = existingTP.Prices.Where(x => !tuitionPartnerToProcess.Prices.Any(e => e.TuitionSettingId == x.TuitionSettingId &&
                                                                                                e.SubjectId == x.SubjectId &&
                                                                                                e.GroupSize == x.GroupSize));
        foreach (var priceToDelete in pricesToDelete)
        {
            dbContext.Entry(priceToDelete).State = EntityState.Deleted;
        }

        foreach (var price in tuitionPartnerToProcess.Prices)
        {
            var existingPrice = existingTP.Prices.FirstOrDefault(x => x.TuitionSettingId == price.TuitionSettingId &&
                                                                    x.SubjectId == price.SubjectId &&
                                                                    x.GroupSize == price.GroupSize);

            if (existingPrice == null)
            {
                existingTP.Prices.Add(price);
            }
            else
            {
                //Update the existing data, will be updated since ref to object
                existingPrice = price.Adapt(existingPrice);
            }
        }
    }

    private async Task DeactivateTPs(NtpDbContext dbContext, List<TuitionPartner> allExistingTPs, CancellationToken cancellationToken)
    {
        var allProcessedTps = GetAllProcessedTPs();

        var tpsToDeactivate = allExistingTPs
                    .Where(x => x.IsActive &&
                                !allProcessedTps.Select(s => s.ImportId.ToLower()).Contains(x.ImportId.ToLower()))
                    .ToList();

        if (tpsToDeactivate != null)
        {
            foreach (var tpToDeactivate in tpsToDeactivate)
            {
                SetImportingData(tpToDeactivate, false);

                await dbContext.SaveChangesAsync(cancellationToken);

                AddToProcessedTPs("Deactivated", tpToDeactivate);
            }
        }
    }

    private static void SetImportingData(TuitionPartner tp, bool isActive = true)
    {
        tp.ImportProcessLastUpdatedData = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        tp.IsActive = isActive;
    }

    private async Task ImportTuitionPartnerLogos(NtpDbContext dbContext, ILogoFileEnumerable logoFileEnumerable, CancellationToken cancellationToken)
    {
        var partners = await dbContext.TuitionPartners
            .Where(x => x.IsActive)
            .Select(x => new { x.Id, x.SeoUrl })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Looking for logos for {Count} tuition partners", partners.Count);

        var logos = logoFileEnumerable.ToList();

        var matching = (from p in partners
                        from l in logos
                        where IsFileLogoForTuitionPartner(p.SeoUrl,
                            l.Filename.ExtractFileNameFromDirectory())
                        select new
                        {
                            Partner = p,
                            Logo = l,
                        })
                       .ToList();

        _logger.LogInformation("Matched {Count} logos to active tuition partners:\n{Matches}",
            matching.Count, string.Join("\n", matching.Select(x => $"{x.Partner.SeoUrl} => {x.Logo.Filename}")));

        var partnersWithoutLogos = partners.Except(matching.Select(x => x.Partner)).ToList();
        if (partnersWithoutLogos.Any())
        {
            _logger.LogWarning("{TPDataImportLogMessagePrefix}Warning - {Count} active tuition partners do not have logos:\n{WithoutLogo}",
                StringConstants.TPDataImportLogMessagePrefix,
                partnersWithoutLogos.Count, string.Join("\n", partnersWithoutLogos.Select(x => x.SeoUrl)));
        }

        var logosWithoutPartners = logos.Except(matching.Select(x => x.Logo)).ToList();
        if (logosWithoutPartners.Any())
        {
            _logger.LogWarning("{TPDataImportLogMessagePrefix}Warning - {Count} logos files do not match an active tuition partner:\n{UnmatchedLogos}",
                StringConstants.TPDataImportLogMessagePrefix,
                logosWithoutPartners.Count, string.Join("\n", logosWithoutPartners.Select(x => x.Filename)));
        }

        foreach (var import in matching)
        {
            _logger.LogInformation("Retrieving logo file for Tuition Partner {Name}", import.Partner.SeoUrl);
            var b64 = Convert.ToBase64String(import.Logo.Stream.Value.ReadAllBytes());

            var tp = dbContext.TuitionPartners.Find(import.Partner.Id);
            tp!.Logo = new TuitionPartnerLogo
            {
                Logo = b64,
                FileExtension = import.Logo.FileExtension
            };
        }

        await dbContext.SaveChangesAsync();

        await Task.CompletedTask;
    }

    public static bool IsFileLogoForTuitionPartner(string tuitionPartnerName, string logoFilename)
        => SupportedImageFormats.FileExtensions
            .Select(ext => $"Logo_{tuitionPartnerName}{ext}")
            .Contains(logoFilename);

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public static class StreamExtensions
{
    public static byte[] ReadAllBytes(this Stream instream)
    {
        if (instream is MemoryStream stream)
            return stream.ToArray();

        using var memoryStream = new MemoryStream();
        instream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}