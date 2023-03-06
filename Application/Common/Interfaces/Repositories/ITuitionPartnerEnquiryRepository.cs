using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerEnquiryRepository : IGenericRepository<TuitionPartnerEnquiry>
{
    Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId, string baseServiceUrl);

    Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(int enquiryId, int tuitionPartnerId);
}