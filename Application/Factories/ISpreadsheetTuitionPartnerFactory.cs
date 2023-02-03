using Domain;

namespace Application.Factories;

public interface ISpreadsheetTuitionPartnerFactory
{
    Task<TuitionPartner> GetTuitionPartner(Stream stream, string filename, IList<Region> regions, IList<Subject> subjects, IList<OrganisationType> organisationTypes,
        IDictionary<string, DateTime> tpImportedDates, CancellationToken cancellationToken);
}