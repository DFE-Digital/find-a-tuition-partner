using Application.Factories;
using Domain;
using Domain.Validators;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public class DataImporterService : IHostedService
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IHostApplicationLifetime _host;
    private readonly DataEncryption _config;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DataImporterService(IHostApplicationLifetime host, IOptions<DataEncryption> config, ILogger<DataImporterService> logger, IServiceScopeFactory scopeFactory)
    {
        _host = host;
        _config = config.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_config.Key))
        {
            _logger.LogError("Importing encrypted files requires the --DataEncryption:Key argument e.g. --DataEncryption:Key \"I0YRt6YZrMvdTSN107O1R5b4lS16Gz7wBMMruEhqAJc=\". These can also be environment variables");

            _host.StopApplication();

            return;
        }

        var keyBytes = Convert.FromBase64String(_config.Key);

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();
        var factory = scope.ServiceProvider.GetRequiredService<ITuitionPartnerFactory>();

        _logger.LogInformation("Migrating database");
        await dbContext.Database.MigrateAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                _logger.LogInformation("Deleting all existing Tuition Partner data");
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocalAuthorityDistrictCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"SubjectCoverage\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Prices\"", cancellationToken: cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"TuitionPartners\"", cancellationToken: cancellationToken);

                var credential = (await GoogleCredential.FromFileAsync("credentials.json", cancellationToken))
                    .CreateScoped(DriveService.Scope.DriveReadonly);

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                });

                // Define parameters of request.
                FilesResource.ListRequest listRequest = service.Files.List();
                // List only files in the tuition partner data folder within the school services shared drive
                listRequest.DriveId = "0ALN4QSSNcSvkUk9PVA";
                listRequest.Q = "parents in '1uda1n7cWHS4goRuDxhJBHoURAlAgh3YB' and trashed = false";
                // Following must be set when searching within a shared drive
                listRequest.IncludeItemsFromAllDrives = true;
                listRequest.SupportsAllDrives = true;
                listRequest.Corpora = "drive";

                listRequest.OrderBy = "name";
                listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
                listRequest.PageSize = 50;

                while (true)
                {
                    var fileList = await listRequest.ExecuteAsync(cancellationToken);
                    var files = fileList.Files;
                    Console.WriteLine("Files:");
                    if (files == null || files.Count == 0)
                    {
                        Console.WriteLine("No files found.");
                        return;
                    }
                    foreach (var file in files)
                    {
                        var originalFilename = file.Name;
                        _logger.LogInformation($"Attempting to import original Tuition Partner spreadsheet {originalFilename}");

                        var stream = new MemoryStream();
                        IDownloadProgress? status;

                        if (file.MimeType == ExcelMimeType)
                        {
                            var request = service.Files.Get(file.Id);
                            status = request.DownloadWithStatus(stream);
                        }
                        else
                        {
                            var exportRequest = service.Files.Export(file.Id, ExcelMimeType);
                            status = exportRequest.DownloadWithStatus(stream);
                        }

                        if (status.Status != DownloadStatus.Completed)
                        {
                            _logger.LogWarning($"Could not download file {originalFilename} from Google Drive. Status {status.Status} bytes downloaded {status.BytesDownloaded} exception {status.Exception}");
                            continue;
                        }

                        _logger.LogInformation($"Attempting create Tuition Partner from file {originalFilename}");
                        TuitionPartner tuitionPartner;
                        try
                        {
                            tuitionPartner = await factory.GetTuitionPartner(stream, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Exception thrown when creating Tuition Partner from file {originalFilename}");
                            continue;
                        }

                        var validator = new TuitionPartnerValidator();
                        var results = await validator.ValidateAsync(tuitionPartner, cancellationToken);
                        if (!results.IsValid)
                        {
                            _logger.LogError($"Tuition Partner name {tuitionPartner.Name} created from file {originalFilename} is not valid.{Environment.NewLine}{string.Join(Environment.NewLine, results.Errors)}");
                            continue;
                        }

                        _logger.LogInformation(tuitionPartner.SeoUrl);

                        dbContext.TuitionPartners.Add(tuitionPartner);
                        await dbContext.SaveChangesAsync(cancellationToken);

                        _logger.LogInformation($"Added Tuition Partner {tuitionPartner.Name} with id of {tuitionPartner.Id} from file {originalFilename}");
                    }

                    if (string.IsNullOrEmpty(fileList.NextPageToken)) break;

                    listRequest.PageToken = fileList.NextPageToken;
                }

                await transaction.CommitAsync(cancellationToken);
            });

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}