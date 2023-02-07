using Application.Repositories;
using Domain;

namespace Infrastructure.Repositories;

public class EnquiryRepository : IEnquiryRepository
{
    private readonly NtpDbContext _dbContext;

    public EnquiryRepository(NtpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddEnquiryAsync(Enquiry enquiry, CancellationToken cancellationToken = default)
    {
        await _dbContext.Enquiries.AddAsync(enquiry, cancellationToken);

        var numberOfRecordsEffected = await _dbContext.SaveChangesAsync(cancellationToken);

        return numberOfRecordsEffected > 0;
    }
}