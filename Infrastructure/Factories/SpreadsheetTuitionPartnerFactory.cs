using Application.Extraction;
using Application.Factories;
using Domain;

namespace Infrastructure.Factories;

public class SpreadsheetTuitionPartnerFactory : ISpreadsheetTuitionPartnerFactory
{
    private readonly ISpreadsheetExtractor _spreadsheetExtractor;
    private readonly IQualityAssuredSpreadsheetTuitionPartnerFactory _qualityAssuredSpreadsheetTuitionPartnerFactory;
    private readonly ITribalSpreadsheetTuitionPartnerFactory _tribalSpreadsheetTuitionPartnerFactory;

    public SpreadsheetTuitionPartnerFactory(ISpreadsheetExtractor spreadsheetExtractor,
        IQualityAssuredSpreadsheetTuitionPartnerFactory qualityAssuredSpreadsheetTuitionPartnerFactory,
        ITribalSpreadsheetTuitionPartnerFactory tribalSpreadsheetTuitionPartnerFactory)
    {
        _spreadsheetExtractor = spreadsheetExtractor;
        _qualityAssuredSpreadsheetTuitionPartnerFactory = qualityAssuredSpreadsheetTuitionPartnerFactory;
        _tribalSpreadsheetTuitionPartnerFactory = tribalSpreadsheetTuitionPartnerFactory;
    }

    public async Task<TuitionPartner> GetTuitionPartner(Stream stream, string filename,
        IList<Region> regions, IList<Subject> subjects, IList<OrganisationType> organisationTypes, CancellationToken cancellationToken)
    {
        _spreadsheetExtractor.SetStream(stream);

        var isTribalSpreadsheet = _spreadsheetExtractor.WorksheetExists(TribalSpreadsheetTuitionPartnerFactory.OrganisationDetailsSheetName);

        //Identify which factory to use...
        if (isTribalSpreadsheet)
        {
            return _tribalSpreadsheetTuitionPartnerFactory.GetTuitionPartner(_spreadsheetExtractor, filename, regions, subjects, organisationTypes);
        }

        return await _qualityAssuredSpreadsheetTuitionPartnerFactory.GetTuitionPartner(_spreadsheetExtractor, organisationTypes, cancellationToken);
    }
}