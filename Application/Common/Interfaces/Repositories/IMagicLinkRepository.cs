using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IMagicLinkRepository : IGenericRepository<MagicLink>
{
    Task<EnquiryResponseModel?> GetEnquirerEnquiryResponseReceivedData(int enquiryId,
        int tuitionPartnerId);
}