using Application.Extraction;
using Domain;

namespace Application.Factories;

public interface IQualityAssuredSpreadsheetTuitionPartnerFactory
{
    Task<TuitionPartner> GetTuitionPartner(ISpreadsheetExtractor spreadsheetExtractor, IList<OrganisationType> organisationTypes, CancellationToken cancellationToken);
}