using System.Globalization;
using Application.Common.DTO;
using Application.Common.Interfaces;
using CsvHelper;
using Domain.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Npgsql;
using AppEnvironmentVariables = Infrastructure.Constants.EnvironmentVariables;

namespace Functions;

public class DataExtraction
{
    private readonly ILogger<DataExtraction> _logger;
    private readonly IConfiguration _configuration;
    private readonly INotificationsClientService _notificationsClientService;

    public DataExtraction(ILogger<DataExtraction> logger, IConfiguration configuration,
        INotificationsClientService notificationsClientService)
    {
        _logger = logger;
        _configuration = configuration;
        _notificationsClientService = notificationsClientService;
    }

    [Function("DataExtraction")]
    public async Task RunAsync([TimerTrigger("%DataExtractionTimerCronExpression%")] MyInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Find a Tuition Partner DataExtraction function started execution at: {DateTime.UtcNow}");

        var dbConnectionString = _configuration.GetConnectionString(AppEnvironmentVariables.FatpDatabaseConnectionString);
        await using var connection = new NpgsqlConnection(dbConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);

            await using var enquiriesCommand = new NpgsqlCommand(DataExtractionQueries.EnquiriesPsqlQuery, connection);

            var enquiriesCommandReader = await enquiriesCommand.ExecuteReaderAsync(cancellationToken);

            byte[] enquiriesByteArray = ConvertDataReaderToCSVByteArray(enquiriesCommandReader);

            await enquiriesCommandReader.CloseAsync();

            await using var enquiriesResponsesCommand =
                new NpgsqlCommand(DataExtractionQueries.EnquiriesResponsesPsqlQuery, connection);

            var enquiriesResponsesCommandReader = await enquiriesResponsesCommand.ExecuteReaderAsync(cancellationToken);

            byte[] enquiriesResponseByteArray = ConvertDataReaderToCSVByteArray(enquiriesResponsesCommandReader);

            await enquiriesResponsesCommandReader.CloseAsync();

            var personalisation = new Dictionary<string, dynamic>
            {
                { "link_to_enquiries_file", NotificationClient.PrepareUpload(enquiriesByteArray, true, true, "2 weeks")},
                { "link_to_enquiries_responses _file", NotificationClient.PrepareUpload(enquiriesResponseByteArray, true, true, "2 weeks")}
            };

            var recipientEmail = _configuration["DataExtractionRecipientEmail"];

            if (string.IsNullOrEmpty(recipientEmail)) throw new ArgumentNullException("DataExtractionRecipientEmail");

            var notifyEmail = new NotifyEmailDto()
            {
                ClientReference = $"DataExtraction-FA-{DateTime.UtcNow}",
                Email = recipientEmail,
                Personalisation = personalisation,
                EmailTemplateType = EmailTemplateType.DataExtraction
            };

            await _notificationsClientService.SendEmailAsync(notifyEmail);

            _logger.LogInformation(
                $"Find a Tuition Partner DataExtraction function finished execution at: {DateTime.UtcNow}");

        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occured while trying to execute the DataExtraction function. Exception: {ex}", ex);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private static byte[] ConvertDataReaderToCSVByteArray(NpgsqlDataReader dr)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(dr); // Writes the data reader to CSV
        writer.Flush();

        return memoryStream.ToArray();
    }
}