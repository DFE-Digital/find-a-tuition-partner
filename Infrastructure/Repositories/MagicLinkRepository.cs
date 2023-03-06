using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Infrastructure.Repositories;

public class MagicLinkRepository : GenericRepository<MagicLink>, IMagicLinkRepository
{
    public MagicLinkRepository(NtpDbContext context) : base(context)
    {
    }
}