using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EmailStatusRepository : GenericRepository<EmailStatus>, IEmailStatusRepository
{
    public EmailStatusRepository(NtpDbContext context) : base(context)
    {
    }
}