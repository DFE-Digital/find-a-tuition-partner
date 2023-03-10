using Application.Common.Interfaces.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class KeyStageSubjectEnquiryRepository : GenericRepository<KeyStageSubjectEnquiry>, IKeyStageSubjectEnquiryRepository
{
    public KeyStageSubjectEnquiryRepository(NtpDbContext context) : base(context)
    {
    }
}