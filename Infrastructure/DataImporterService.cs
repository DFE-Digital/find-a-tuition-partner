using Application.DataImport;
using Application.Factories;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain;
using Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        var factory = scope.ServiceProvider.GetRequiredService<ITuitionPartnerFactory>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                //await RemoveTuitionPartners(dbContext, cancellationToken);

                //await ImportTuitionPartnerFiles(dbContext, dataFileEnumerable, factory, cancellationToken);

                await ImportTutionPartnerLogos(dbContext, logoFileEnumerable, factory, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            });

        _host.StopApplication();
    }

    private async Task RemoveTuitionPartners(NtpDbContext dbContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all existing Tuition Partner data");
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);
    }

    private async Task ImportTuitionPartnerFiles(NtpDbContext dbContext, IDataFileEnumerable dataFileEnumerable, ITuitionPartnerFactory factory, CancellationToken cancellationToken)
    {
        foreach (var dataFile in dataFileEnumerable)
        {
            var originalFilename = dataFile.Filename;

            _logger.LogInformation("Attempting to create Tuition Partner from file {OriginalFilename}", originalFilename);
            TuitionPartner tuitionPartner;
            try
            {
                tuitionPartner = await factory.GetTuitionPartner(dataFile.Stream, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown when creating Tuition Partner from file {OriginalFilename}", originalFilename);
                continue;
            }

            var validator = new TuitionPartnerValidator();
            var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
            if (!results.IsValid)
            {
                _logger.LogError($"Tuition Partner name {{TuitionPartnerName}} created from file {{originalFilename}} is not valid.{Environment.NewLine}{{Errors}}",
                    tuitionPartner.Name, originalFilename, string.Join(Environment.NewLine, results.Errors));
                continue;
            }

            dbContext.TuitionPartners.Add(tuitionPartner);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added Tuition Partner {TuitionPartnerName} with id of {TuitionPartnerId} from file {OriginalFilename}",
                tuitionPartner.Name, tuitionPartner.Id, originalFilename);
        }
    }

    private async Task ImportTutionPartnerLogos(NtpDbContext dbContext, ILogoFileEnumerable logoFileEnumerable, ITuitionPartnerFactory factory, CancellationToken cancellationToken)
    {
        var partners = await dbContext.TuitionPartners.Select(x => new { x.Id, x.SeoUrl }).ToListAsync(cancellationToken);
        var logos = logoFileEnumerable.ToList();
        foreach (var partner in partners)
        {
            foreach (var dataFile in logos)
            {
                if (!dataFile.Filename.Contains(partner.SeoUrl)) continue;

                var b64 = "PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNTIuOTQxIDEwMCI+CiAgICA8ZGVmcz4KICAgICAgICA8Y2xpcFBhdGggaWQ9ImFsb2dvIj4KICAgICAgICAgICAgPHBhdGggZGF0YS1uYW1lPSJQYXRoIDQ1IiBkPSJNMC0xMS43MzJoMjUyLjk0MXYtMTAwSDBaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgwIDExMS43MzIpIj48L3BhdGg+CiAgICAgICAgPC9jbGlwUGF0aD4KICAgIDwvZGVmcz4KICAgIDxnIGRhdGEtbmFtZT0iR3JvdXAgMzQ0NiI+CiAgICAgICAgPGcgZGF0YS1uYW1lPSJHcm91cCAxNyIgY2xpcC1wYXRoPSJ1cmwoI2Fsb2dvKSI+CiAgICAgICAgICAgIDxnIGRhdGEtbmFtZT0iR3JvdXAgMTUiPgogICAgICAgICAgICAgICAgPHBhdGggZGF0YS1uYW1lPSJQYXRoIDQzIiBkPSJNLjQ4NyA4My41MThWOTkuNzloMTMuNTM0di0zLjcxNEg1LjMxNlY5My41SDEzdi0zLjcxNUg1LjMxNnYtMi41NTFoOC40NzF2LTMuNzE2Wm0zNi45MTEgMGgtNy4xMjdWOTkuNzloNi45NjVjNS4zNjIgMCA4LjkxNC0zLjI1IDguOTE0LTguMTQ4cy0zLjQ4My04LjEyNC04Ljc1Mi04LjEyNG0uMTQgMTIuNDloLTIuNDM5di04LjY4M2gyLjIwNWMyLjMyMiAwIDMuOTI0IDEuNzQxIDMuOTI0IDQuMzY0IDAgMi42LTEuNDg2IDQuMzE4LTMuNjkxIDQuMzE4bTI4LjkyNS0zLjA4N3YtOS40aC00LjgyOXY5LjRjMCA0LjMxNyAzLjA0MSA3LjA4IDcuNzA4IDcuMDggNC42NDIgMCA3LjU5LTIuNzYzIDcuNTktNy4wOHYtOS40aC00LjgyOHY5LjRhMi43MjIgMi43MjIgMCAwIDEtMi43NCAzLjA0MSAyLjg0MiAyLjg0MiAwIDAgMS0yLjktMy4wNDFtMzguNjUyLTMuMzY2IDIuNzg1LTMuM2E5LjU2NSA5LjU2NSAwIDAgMC02LjczMy0yLjg3NmMtNS4wMzcgMC04Ljc1MSAzLjUwNi04Ljc1MSA4LjI0MWE4LjI1NiA4LjI1NiAwIDAgMCA4LjU4OSA4LjM4MSA5LjgzNiA5LjgzNiAwIDAgMCA2Ljg5NC0zLjEzNGwtMi43ODUtMi45NzFhNS44ODkgNS44ODkgMCAwIDEtMy44NzcgMS44OCAzLjk2NiAzLjk2NiAwIDAgMS0zLjg3Ny00LjIgMy45NDUgMy45NDUgMCAwIDEgMy44NzctNC4xNzggNS4zIDUuMyAwIDAgMSAzLjg3NyAyLjE1OW0yOS44NzMgMTAuMjM0aDUuMTUzbC02LjcwNS0xNi4yNzJoLTQuOTY4bC02Ljk0IDE2LjI3Mmg0Ljk2N2wuOTc2LTIuNmg2LjU2OVptLTYuMTc1LTYuMTI4IDItNS4zMTYgMS45NSA1LjMxNlptMjMuNC0xMC4xNDV2My44NTRoNC43MTJWOTkuNzloNC44NTJWODcuMzcxaDQuNzM1di0zLjg1M1ptMjkuNzM3IDE2LjI3M2g0LjgyOVY4My41MThoLTQuODI5Wm0yOS40MzgtMTYuNDEyYy01LjEwOCAwLTguODY4IDMuNDgyLTguODY4IDguMjY0IDAgNC44MDUgMy43NiA4LjM1NyA4Ljg2OCA4LjM1N3M4Ljg2OC0zLjU3NiA4Ljg2OC04LjM1N2MwLTQuNzU5LTMuNzYxLTguMjY0LTguODY4LTguMjY0bS4wNyA0LjA0YTMuOTkgMy45OSAwIDAgMSAzLjg1MyA0LjI0OCA0LjAzMiA0LjAzMiAwIDAgMS0zLjg1MyA0LjMgNC4xNDEgNC4xNDEgMCAwIDEtMy45OTMtNC4zIDQuMDg0IDQuMDg0IDAgMCAxIDMuOTkzLTQuMjQ4bTI0LjUzNy0zLjlWOTkuNzloNC40MzR2LTkuMDU2bDYuODk0IDkuMDUzaDQuMTA5VjgzLjUxOGgtNC40MTF2OS4xbC02Ljg5NC05LjFaIj48L3BhdGg+CiAgICAgICAgICAgIDwvZz4KICAgICAgICAgICAgPGcgZGF0YS1uYW1lPSJHcm91cCAxNiI+CiAgICAgICAgICAgICAgICA8cGF0aCBkYXRhLW5hbWU9IlBhdGggNDQiIGQ9Ik0yMS45MTQgNjUuNzQzaDEwLjQ5M3YtNjVoLTkuMVYzOS42NUwxMi44MTQuNzQzSDB2NjVoOS4xOTNWMTguNjY0Wm0zNS45MzQtNjV2NDkuNDkzYzAgMTAuNCA1LjIgMTYuMzQ0IDE1LjIyOSAxNi4zNDRzMTUuMjI5LTUuOTQ0IDE1LjIyOS0xNi4zNDVWLjc0M2gtOS42NTd2NTAuMTQzYzAgNC42NDItMi4wNDMgNi4zMTQtNS4yOTMgNi4zMTRzLTUuMjkzLTEuNjcyLTUuMjkzLTYuMzE0Vi43NDNabTU1Ljk5MyA2NWgxNi4xNTdjMTAuMjE1IDAgMTUuMjI4LTUuNjY0IDE1LjIyOC0xNi4wNjRWMTYuODA3YzAtMTAuNC01LjAxNC0xNi4wNjQtMTUuMjI4LTE2LjA2NGgtMTYuMTU3Wm0xNS45NzEtNTUuNzE1YzMuMjUxIDAgNS4yIDEuNjcyIDUuMiA2LjMxNHYzMy44YzAgNC42NDItMS45NDkgNi4zMTQtNS4yIDYuMzE0aC01Ljc1NlYxMC4wMjhabTU1LjgwNyAyOC43ODZoNC42NDN2MTEuOTc5YzAgNC42NDMtMi4wNDMgNi4zMTQtNS4yOTMgNi4zMTRzLTUuMjkzLTEuNjcxLTUuMjkzLTYuMzE0di0zNS4xYzAtNC42NDMgMi4wNDItNi40MDcgNS4yOTMtNi40MDdzNS4yOTMgMS43NjQgNS4yOTMgNi40MDd2Ni45NjNoOS42NTd2LTYuMzE0YzAtMTAuNC01LjItMTYuMzQzLTE1LjIyOS0xNi4zNDNzLTE1LjIyOCA1Ljk0My0xNS4yMjggMTYuMzQzdjMzLjhjMCAxMC40IDUuMiAxNi4zNDMgMTUuMjI4IDE2LjM0M3MxNS4yMjktNS45NDMgMTUuMjI5LTE2LjM0M1YyOS41MjloLTE0LjNabTQ5LjY3OS0yOC43ODZoMTcuNjQzVi43NDJoLTI3Ljg1NnY2NWgyNy44NTh2LTkuMjg2aC0xNy42NDVWMzcuNDIyaDE0LjAyMXYtOS4yODZoLTE0LjAyMVoiPjwvcGF0aD4KICAgICAgICAgICAgPC9nPgogICAgICAgIDwvZz4KICAgIDwvZz4KPC9zdmc+";

                var tp = dbContext.TuitionPartners.Find(partner.Id);
                tp!.Logo = b64;
            }
        }

        await dbContext.SaveChangesAsync();

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}