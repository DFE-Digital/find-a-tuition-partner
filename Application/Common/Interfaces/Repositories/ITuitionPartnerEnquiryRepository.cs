using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerEnquiryRepository : IGenericRepository<TuitionPartnerEnquiry>
{
    Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(int enquiryId, int tuitionPartnerId);
    Task<EnquirerViewTutionPartnerDetailsModel?> GetEnquirerViewTutionPartnerDetailsResponse(int enquiryId, int tuitionPartnerId);
}