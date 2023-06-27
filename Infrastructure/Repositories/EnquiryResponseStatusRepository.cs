using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EnquiryResponseStatusRepository : GenericRepository<EnquiryResponseStatus>, IEnquiryResponseStatusRepository
{
    public EnquiryResponseStatusRepository(NtpDbContext context) : base(context)
    {
    }
}