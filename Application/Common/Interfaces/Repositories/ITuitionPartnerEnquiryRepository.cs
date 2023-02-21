using Application.Common.Models;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface ITuitionPartnerEnquiryRepository : IGenericRepository<TuitionPartnerEnquiry>
{
    Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId);
}