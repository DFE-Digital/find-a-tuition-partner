using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EnquiryRepository : GenericRepository<Enquiry>, IEnquiryRepository
{
    public EnquiryRepository(NtpDbContext context) : base(context)
    {
    }
}