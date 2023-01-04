using Application.Extraction;
using Application.Factories;
using Domain;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Factories;

public class SpreadsheetTuitionPartnerFactory : ITuitionPartnerFactory
{
    private readonly ILogger<SpreadsheetTuitionPartnerFactory> _logger;
    private readonly ISpreadsheetExtractor _spreadsheetExtractor;
    private readonly NtpDbContext _dbContext;

    public SpreadsheetTuitionPartnerFactory(ILogger<SpreadsheetTuitionPartnerFactory> logger, ISpreadsheetExtractor spreadsheetExtractor, NtpDbContext dbContext)
    {
        _logger = logger;
        _spreadsheetExtractor = spreadsheetExtractor;
        _dbContext = dbContext;
    }

    public async Task<TuitionPartner> GetTuitionPartner(Stream stream, string filename, CancellationToken cancellationToken)
    {
        _spreadsheetExtractor.SetStream(stream);

        //TODO - decide best way to identify the spreadsheet
        var isTribalSpreadsheet = _spreadsheetExtractor.WorksheetExists(TribalSpreadsheetTuitionPartnerFactory.OrganisationDetailsSheetName);

        //Identify which factory to use, with the strategy pattern
        ITuitionPartnerFactoryStrategy tuitionPartnerFactoryStrategy = isTribalSpreadsheet ?
            new TribalSpreadsheetTuitionPartnerFactory(_logger, filename, _dbContext) :
            new QualityAssuredSpreadsheetTuitionPartnerFactory(_dbContext);

        return await tuitionPartnerFactoryStrategy.GetTuitionPartner(_spreadsheetExtractor, cancellationToken);
    }
}