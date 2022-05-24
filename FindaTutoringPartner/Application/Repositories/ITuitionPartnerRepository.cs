using Domain.Deltas;

namespace Application.Repositories;

public interface ITuitionPartnerRepository
{
    Task ApplyDeltas(TuitionPartnerDeltas deltas);
}