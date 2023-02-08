using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class SchoolRepository : GenericRepository<School>, ISchoolRepository
{
    public SchoolRepository(NtpDbContext dbContext) : base(dbContext)
    {
    }
}