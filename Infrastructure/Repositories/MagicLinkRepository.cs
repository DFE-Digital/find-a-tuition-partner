using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class MagicLinkRepository : GenericRepository<MagicLink>, IMagicLinkRepository
{
    public MagicLinkRepository(NtpDbContext context) : base(context)
    {
    }
}