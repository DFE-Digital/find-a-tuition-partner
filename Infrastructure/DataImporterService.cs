using System.ComponentModel.DataAnnotations;
using Application;
using Application.DataImport;
using Application.Factories;
using Application.Mapping;
using Domain;
using Domain.Validators;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(IHostApplicationLifetime host, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var dataFileEnumerable = scope.ServiceProvider.GetRequiredService<IDataFileEnumerable>();
        var logoFileEnumerable = scope.ServiceProvider.GetRequiredService<ILogoFileEnumerable>();
        var factory = scope.ServiceProvider.GetRequiredService<ISpreadsheetTuitionPartnerFactory>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                await ImportTuitionPartnerFiles(dbContext, dataFileEnumerable, factory, cancellationToken);

                await ImportTutionPartnerLogos(dbContext, logoFileEnumerable, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });


        var generalInformatioAboutSchoolsRecords = scope.ServiceProvider.GetRequiredService<IGeneralInformationAboutSchoolsRecords>();
        var giasFactory = scope.ServiceProvider.GetRequiredService<ISchoolsFactory>();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                await RemoveGeneralInformationAboutSchools(dbContext, cancellationToken);

                await ImportGeneralInformationAboutSchools(dbContext, generalInformatioAboutSchoolsRecords, giasFactory, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });

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
        foreach (SchoolDatum schoolDatum in result.Result.Where(s => s.IsValidForService()))
        {
            var establishmentName = schoolDatum.Name;

            School school;
            try
            {
                school = giasFactory.GetSchool(schoolDatum, localAuthorityDistrictsIds, localAuthorityIds);
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
                _logger.LogInformation($"OPEN Establishment name {{TuitionPartnerName}} {{EstablishmentStatus}} {{EstablishmentTypeGroupId}} {{EstablishmentType}} General Information About Schools created from recoord {{originalFilename}} is not valid.{Environment.NewLine}{{Errors}}",
                        school.EstablishmentName, school.EstablishmentStatusId, school.EstablishmentTypeGroupId, schoolDatum.EstablishmentType, school.EstablishmentName, string.Join(Environment.NewLine, results.Errors));
                failedValidation++;
                continue;
            }

            dbContext.Schools.Add(school);

            imported++;
            if ((imported % 100) == 0)
            {
                _logger.LogInformation("Imported {Count} schools from GIAS dataset", imported);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully imported {Count} valid schools from GIAS dataset. {FailedValidationCount} schools failed validation", imported, failedValidation);
    }

    private async Task RemoveGeneralInformationAboutSchools(NtpDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all existing General Information About Schools data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Schools\"", cancellationToken: cancellationToken);
    }

    private async Task ImportTuitionPartnerFiles(NtpDbContext dbContext, IDataFileEnumerable dataFileEnumerable, ISpreadsheetTuitionPartnerFactory factory,
        CancellationToken cancellationToken)
    {
        var successfullyProcessed = new Dictionary<string, TuitionPartner>();

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

        _logger.LogInformation("Number of existing TPs {AllExistingTPsCount}", allExistingTPs.Count);

        var tpImportedDates = allExistingTPs
                .Select(x => new { x.Name, x.TPLastUpdatedData })
                .ToDictionary(x => x.Name.ToLower(), x => x.TPLastUpdatedData);

        ConfigurerMapper();

        foreach (var dataFile in dataFileEnumerable)
        {
            var originalFilename = dataFile.Filename;

            //Use Polly to retry up to 5 times per file read (in case of network issues)
            int numberOfRetries = 5;
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(numberOfRetries, retryAttempt =>
                    //Wait 2, 4, 8, 16 and then 32 seconds
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, sleepDuration, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Exception thrown when reading Tuition Partner from file {OriginalFilename}.  Retrying in {SleepDuration}. Attempt {RetryCount} out of {NumberOfRetries}", originalFilename, sleepDuration, retryCount, numberOfRetries);
                    });


            _logger.LogInformation("Attempting to create Tuition Partner from file {OriginalFilename}", originalFilename);
            TuitionPartner tuitionPartnerToProcess = new();
            try
            {
                tuitionPartnerToProcess = await retryPolicy.ExecuteAsync(async () =>
                {
                    //Uncomment to test polly
                    //Random random = new();
                    //if (random.Next(1, 3) == 1)
                    //    throw new Exception("Testing Polly");

                    return await factory.GetTuitionPartner(dataFile.Stream.Value, originalFilename, regions, subjects, organisationTypes, tpImportedDates, cancellationToken);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when creating Tuition Partner from file {OriginalFilename}", originalFilename);
                //Errors reading any file then throw exception, so cancels the transaction and rollsback db.
                throw;
            }
            _logger.LogInformation("MessageTest1 - {OriginalFilename}", originalFilename);
            var validator = new TuitionPartnerValidator(successfullyProcessed);
            var results = await validator.ValidateAsync(tuitionPartnerToProcess, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogInformation("MessageTest2 - {OriginalFilename}", originalFilename);
                var errorMsg = $"Tuition Partner name {tuitionPartnerToProcess.Name} created from file {originalFilename} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}";
                _logger.LogError(message: errorMsg);
                //Errors validating any file then throw exception, so cancels the transaction and rollsback db.
                throw new ValidationException(errorMsg);
            }
            _logger.LogInformation("MessageTest3 - {OriginalFilename}", originalFilename);
            //TODO - test duplicate import id & seo url in files


            tuitionPartnerToProcess.IsActive = true;

            var matchedTPs = allExistingTPs
                    .Where(x => x.ImportId == tuitionPartnerToProcess.ImportId ||
                                x.SeoUrl == tuitionPartnerToProcess.SeoUrl)
                    .ToList();
            _logger.LogInformation("MessageTest4 - {OriginalFilename}", originalFilename);
            if (matchedTPs == null)
            {
                _logger.LogInformation("MessageTest5 - {OriginalFilename}", originalFilename);
                //TODO - test this
                tuitionPartnerToProcess.ImportProcessLastUpdatedData = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                dbContext.TuitionPartners.Add(tuitionPartnerToProcess);

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Added Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId} from file {OriginalFilename}",
                    tuitionPartnerToProcess.Name, tuitionPartnerToProcess.Id, originalFilename);
            }
            else if (matchedTPs.Count == 1)
            {
                _logger.LogInformation("MessageTest6 - {OriginalFilename}", originalFilename);
                var existingTP = matchedTPs.First();

                existingTP = tuitionPartnerToProcess!.Adapt(existingTP);

                ImportTuitionPartnerLocalAuthorityDistrictCoverage(dbContext, existingTP, tuitionPartnerToProcess);
                ImportTuitionPartnerSubjectCoverage(dbContext, existingTP, tuitionPartnerToProcess);
                ImportTuitionPartnerPrices(dbContext, existingTP, tuitionPartnerToProcess);

                //TODO - performace test this
                //TODO - test against live data
                //TODO - test switch org type

                if (dbContext.ChangeTracker.Entries().Any(e => e.State == EntityState.Modified ||
                                                                e.State == EntityState.Added ||
                                                                e.State == EntityState.Deleted))
                {
                    existingTP.ImportProcessLastUpdatedData = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Updated Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId} from file {OriginalFilename}",
                        existingTP.Name, existingTP.Id, originalFilename);
                }
                else
                {
                    _logger.LogInformation("No changes for Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId} from file {OriginalFilename}",
                        existingTP.Name, existingTP.Id, originalFilename);
                }
            }
            else if (matchedTPs.Count > 1)
            {
                _logger.LogInformation("MessageTest7 - {OriginalFilename}", originalFilename);
                //TODO - test this
                var errorMsg = $"The file {originalFilename} with seo url('{tuitionPartnerToProcess.SeoUrl}') and import id('{tuitionPartnerToProcess.ImportId}') is returning more than 1 result from the database.";
                _logger.LogError(message: errorMsg);
                throw new InvalidOperationException(errorMsg);
            }

            successfullyProcessed.Add(originalFilename, tuitionPartnerToProcess);
        }

        var tpsToDeactivate = allExistingTPs
                    .Where(x => x.IsActive &&
                                !successfullyProcessed.Select(s => s.Value.ImportId).Contains(x.ImportId))
                    .ToList();

        if (tpsToDeactivate != null)
        {
            foreach (var tpToDeactivate in tpsToDeactivate)
            {
                tpToDeactivate.IsActive = false;
                tpToDeactivate.ImportProcessLastUpdatedData = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deactivated Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId}",
                    tpToDeactivate.Name, tpToDeactivate.Id);
            }
        }

        //TODO - Update all places currently calls to get TP so that deactiavted are excluded
        //TODO - Add a show-all query string flag to TP page - show active flag, updated dates etc
    }

    private void ConfigurerMapper()
    {
        TypeAdapterConfig<LocalAuthorityDistrictCoverage, LocalAuthorityDistrictCoverage>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.TuitionPartnerId)
            .Ignore(dest => dest.TuitionPartner!)
            .Ignore(dest => dest.TuitionType!)
            .Ignore(dest => dest.LocalAuthorityDistrict!);

        TypeAdapterConfig<SubjectCoverage, SubjectCoverage>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.TuitionPartnerId)
            .Ignore(dest => dest.TuitionPartner!)
            .Ignore(dest => dest.TuitionType!)
            .Ignore(dest => dest.Subject!);

        TypeAdapterConfig<Price, Price>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.TuitionPartnerId)
            .Ignore(dest => dest.TuitionPartner!)
            .Ignore(dest => dest.TuitionType!)
            .Ignore(dest => dest.Subject!);

        TypeAdapterConfig<TuitionPartner, TuitionPartner>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Prices!)
            .Ignore(dest => dest.LocalAuthorityDistrictCoverage!)
            .Ignore(dest => dest.SubjectCoverage!)
            .Ignore(dest => dest.Logo!)
            .Ignore(dest => dest.HasLogo)
            .Ignore(dest => dest.OrganisationType);
    }

    private static void ImportTuitionPartnerLocalAuthorityDistrictCoverage(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var laDistrictCoveragesToDelete = existingTP.LocalAuthorityDistrictCoverage.Where(x => !tuitionPartnerToProcess.LocalAuthorityDistrictCoverage.Any(e => e.TuitionTypeId == x.TuitionTypeId &&
                                                                                                                                                e.LocalAuthorityDistrictId == x.LocalAuthorityDistrictId));
        foreach (var laDistrictCoverageToDelete in laDistrictCoveragesToDelete)
        {
            dbContext.Entry(laDistrictCoverageToDelete).State = EntityState.Deleted;
        }

        foreach (var laDistrictCoverage in tuitionPartnerToProcess.LocalAuthorityDistrictCoverage)
        {
            var existingLaDistrictCoverage = existingTP.LocalAuthorityDistrictCoverage.FirstOrDefault(x => x.TuitionTypeId == laDistrictCoverage.TuitionTypeId &&
                                                                                                        x.LocalAuthorityDistrictId == laDistrictCoverage.LocalAuthorityDistrictId);

            if (existingLaDistrictCoverage == null)
            {
                existingTP.LocalAuthorityDistrictCoverage.Add(laDistrictCoverage);
            }
            else
            {
                existingLaDistrictCoverage = laDistrictCoverage.Adapt(existingLaDistrictCoverage);
            }
        }
    }

    private static void ImportTuitionPartnerSubjectCoverage(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var subjectCoveragesToDelete = existingTP.SubjectCoverage.Where(x => !tuitionPartnerToProcess.SubjectCoverage.Any(e => e.TuitionTypeId == x.TuitionTypeId &&
                                                                                                                            e.SubjectId == x.SubjectId));
        foreach (var subjectCoverageToDelete in subjectCoveragesToDelete)
        {
            dbContext.Entry(subjectCoverageToDelete).State = EntityState.Deleted;
        }

        foreach (var subjectCoverage in tuitionPartnerToProcess.SubjectCoverage)
        {
            var existingSubjectCoverage = existingTP.SubjectCoverage.FirstOrDefault(x => x.TuitionTypeId == subjectCoverage.TuitionTypeId &&
                                                                                x.SubjectId == subjectCoverage.SubjectId);

            if (existingSubjectCoverage == null)
            {
                existingTP.SubjectCoverage.Add(subjectCoverage);
            }
            else
            {
                existingSubjectCoverage = subjectCoverage.Adapt(existingSubjectCoverage);
            }
        }
    }

    private static void ImportTuitionPartnerPrices(NtpDbContext dbContext, TuitionPartner existingTP, TuitionPartner tuitionPartnerToProcess)
    {
        var pricesToDelete = existingTP.Prices.Where(x => !tuitionPartnerToProcess.Prices.Any(e => e.TuitionTypeId == x.TuitionTypeId &&
                                                                                                e.SubjectId == x.SubjectId &&
                                                                                                e.GroupSize == x.GroupSize));
        foreach (var priceToDelete in pricesToDelete)
        {
            dbContext.Entry(priceToDelete).State = EntityState.Deleted;
        }

        foreach (var price in tuitionPartnerToProcess.Prices)
        {
            var existingPrice = existingTP.Prices.FirstOrDefault(x => x.TuitionTypeId == price.TuitionTypeId &&
                                                                    x.SubjectId == price.SubjectId &&
                                                                    x.GroupSize == price.GroupSize);

            if (existingPrice == null)
            {
                existingTP.Prices.Add(price);
            }
            else
            {
                existingPrice = price.Adapt(existingPrice);
            }
        }
    }

    private async Task ImportTutionPartnerLogos(NtpDbContext dbContext, ILogoFileEnumerable logoFileEnumerable, CancellationToken cancellationToken)
    {
        var partners = await dbContext.TuitionPartners
            .Where(x => x.IsActive)
            .Select(x => new { x.Id, x.SeoUrl })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Looking for logos for {Count} tuition partners", partners.Count);

        var logos = logoFileEnumerable.ToList();

        var matching = (from p in partners
                        from l in logos
                        where IsFileLogoForTuitionPartner(p.SeoUrl, l.Filename)
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
            _logger.LogInformation("{Count} active tuition partners do not have logos:\n{WithoutLogo}",
                partnersWithoutLogos.Count, string.Join("\n", partnersWithoutLogos.Select(x => x.SeoUrl)));
        }

        var logosWithoutPartners = logos.Except(matching.Select(x => x.Logo)).ToList();
        if (logosWithoutPartners.Any())
        {
            _logger.LogWarning("{Count} logos files do not match an active tuition partner:\n{UnmatchedLogos}",
                logosWithoutPartners.Count, string.Join("\n", logosWithoutPartners.Select(x => x.Filename)));
        }

        foreach (var import in matching)
        {
            _logger.LogInformation("Retrieving logo file for Tution Partner {Name}", import.Partner.SeoUrl);
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