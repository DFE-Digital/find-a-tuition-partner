using Application.Common.Models.Enquiry.Manage;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerEnquiryRepository : IGenericRepository<TuitionPartnerEnquiry>
{
    Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(string supportReferenceNumber, string magicLinkToken);
    Task<EnquirerViewTuitionPartnerDetailsModel?> GetEnquirerViewTuitionPartnerDetailsResponse(string supportReferenceNumber, string magicLinkToken);
}