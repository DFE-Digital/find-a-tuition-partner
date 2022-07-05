using Domain;

namespace Application.Factories;

public interface ITuitionPartnerFactory
{
    TuitionPartner GetTuitionPartner(Stream stream);
}