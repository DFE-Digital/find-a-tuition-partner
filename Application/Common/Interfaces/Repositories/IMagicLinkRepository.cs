using Application.Common.DTO;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IMagicLinkRepository : IGenericRepository<MagicLink>
{
    Task<EnquirerEnquiryResponseReceivedDto?>
        GetEnquirerEnquiryResponseReceivedData(int enquiryId, int tuitionPartnerId);
}