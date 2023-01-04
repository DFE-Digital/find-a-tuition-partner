using Domain;

namespace Application.Factories;

public interface ITuitionPartnerFactory
{
    Task<TuitionPartner> GetTuitionPartner(Stream stream, string filename, CancellationToken cancellationToken);
}