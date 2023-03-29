using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerEnquiryRepository : IGenericRepository<TuitionPartnerEnquiry>
{
    Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(string supportReferenceNumber, string magicLinkToken);
    Task<EnquirerViewTuitionPartnerDetailsModel?> GetEnquirerViewTuitionPartnerDetailsResponse(string supportReferenceNumber, string magicLinkToken);
}