using System.Dynamic;
using System.Globalization;
using Application.Common.Interfaces;
using Azure.Storage.Blobs;
using CsvHelper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using AppEnvironmentVariables = Infrastructure.Constants.EnvironmentVariables;

namespace Functions;

public class DataExtraction
{
    private readonly ILogger<DataExtraction> _logger;
    private readonly IConfiguration _configuration;
    private readonly BlobStorageEnquiriesDataSettings _blobConfig;
    private readonly IGenerateUserDelegationSasTokenAsync _generateUserDelegationSasToken;

    public DataExtraction(ILogger<DataExtraction> logger, IConfiguration configuration,
        IOptions<BlobStorageEnquiriesDataSettings> blobConfig,
        IGenerateUserDelegationSasTokenAsync generateUserDelegationSasToken)
    {
        _logger = logger;
        _configuration = configuration;
        _blobConfig = blobConfig.Value;
        _generateUserDelegationSasToken = generateUserDelegationSasToken;
    }

    [Function("DataExtraction")]
    public async Task RunAsync([TimerTrigger("%DataExtractionTimerCronExpression%")] MyInfo myTimer, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Find a Tuition Partner DataExtraction function started execution at: {DateTime.UtcNow}");

        var sasToken = await _generateUserDelegationSasToken.GenerateUserDelegationSasTokenAsync();

        // Create a BlobServiceClient with the SAS token
        var blobServiceClient =
            new BlobServiceClient(new Uri($"https://{_blobConfig.AccountName}.blob.core.windows.net?{sasToken}"));

        var dbConnectionString = _configuration.GetConnectionString(AppEnvironmentVariables.FatpDatabaseConnectionString);
        await using var connection = new NpgsqlConnection(dbConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);

            // Generate the enquiries.csv and upload it to the blob storage logic. 

            await using var enquiriesCommand = new NpgsqlCommand(DataExtractionQueries.EnquiriesPsqlQuery, connection);

            var enquiriesCommandReader = await enquiriesCommand.ExecuteReaderAsync(cancellationToken);

            var enquiriesByteArray = ConvertDataReaderToCsvByteArray(enquiriesCommandReader);

            // Do not change the CSV file name; it references the logic app, which expects the file name with this exact name.  
            await UploadToBlobStorage(enquiriesByteArray, "enquiries.csv", blobServiceClient); // Upload to blob storage

            await enquiriesCommandReader.CloseAsync();

            // Generate the responses.csv and upload it to the blob storage logic. 

            await using var enquiriesResponsesCommand =
                new NpgsqlCommand(DataExtractionQueries.EnquiriesResponsesPsqlQuery, connection);

            var enquiriesResponsesCommandReader = await enquiriesResponsesCommand.ExecuteReaderAsync(cancellationToken);

            var enquiriesResponseByteArray = ConvertDataReaderToCsvByteArray(enquiriesResponsesCommandReader);

            // Do not change the CSV file name; it references the logic app, which expects the file name with this exact name.  
            await UploadToBlobStorage(enquiriesResponseByteArray, "responses.csv", blobServiceClient); // Upload to blob storage

            await enquiriesResponsesCommandReader.CloseAsync();

            // Generate the tPLaLadRegionsTS.csv and upload it to the blob storage logic. 

            await using var tPLaLadRegionsTSCommand =
                new NpgsqlCommand(DataExtractionQueries.TPLaLadRegionsTSPsqlQuery, connection);

            var tPLaLadRegionsTSCommandReader = await tPLaLadRegionsTSCommand.ExecuteReaderAsync(cancellationToken);

            var tPLaLadRegionsTSByteArray = ConvertDataReaderToCsvByteArray(tPLaLadRegionsTSCommandReader);

            // Do not change the CSV file name; it references the logic app, which expects the file name with this exact name.  
            await UploadToBlobStorage(tPLaLadRegionsTSByteArray, "tPLaLadRegionsTS.csv", blobServiceClient); // Upload to blob storage

            await tPLaLadRegionsTSCommandReader.CloseAsync();

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

    private static byte[] ConvertDataReaderToCsvByteArray(NpgsqlDataReader dr)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        var records = new List<dynamic>();

        while (dr.Read())
        {
            dynamic obj = new ExpandoObject();
            var objectDictionary = (IDictionary<string, object>)obj;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                objectDictionary[dr.GetName(i)] = dr.GetValue(i);
            }

            records.Add(obj);
        }

        csv.WriteRecords(records);
        writer.Flush();

        return memoryStream.ToArray();
    }

    private async Task UploadToBlobStorage(byte[] byteArray, string blobName, BlobServiceClient inputBlobServiceClient)
    {
        var blobContainerClient = inputBlobServiceClient.GetBlobContainerClient(_blobConfig.ContainerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(byteArray);
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}