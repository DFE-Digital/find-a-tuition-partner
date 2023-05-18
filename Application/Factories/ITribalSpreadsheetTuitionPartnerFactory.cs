using Domain;

namespace Application.Factories;

public interface ITribalSpreadsheetTuitionPartnerFactory
{
    List<TuitionPartner> GetTuitionPartners(Stream stream, string filename, IList<Region> regions, IList<Subject> subjects,
        IList<OrganisationType> organisationTypes, IDictionary<string, DateTime> tpImportedDates);
}