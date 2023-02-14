using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EnquiryResponseRepository : GenericRepository<EnquiryResponse>, IEnquiryResponseRepository
{
    public EnquiryResponseRepository(NtpDbContext context) : base(context)
    {
    }
}