using Domain;

namespace Application.Repositories;

public interface IEnquiryRepository
{
    Task<bool> AddEnquiryAsync(Enquiry enquiry, CancellationToken cancellationToken = default);
}