using Domain;

namespace Application.Factories;

public interface ITuitionPartnerFactory
{
    Task<TuitionPartner> GetTuitionPartner(Stream stream, IList<OrganisationType> organisationTypes, CancellationToken cancellationToken);
}