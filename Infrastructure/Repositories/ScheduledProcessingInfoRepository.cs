using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class ScheduledProcessingInfoRepository : GenericRepository<ScheduledProcessingInfo>, IScheduledProcessingInfoRepository
{
    public ScheduledProcessingInfoRepository(NtpDbContext context) : base(context)
    {
    }
}