using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EmailLogRepository : GenericRepository<EmailLog>, IEmailLogRepository
{
    public EmailLogRepository(NtpDbContext context) : base(context)
    {
    }
}