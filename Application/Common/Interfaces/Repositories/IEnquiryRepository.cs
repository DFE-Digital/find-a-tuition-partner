using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IEnquiryRepository : IGenericRepository<Enquiry>
{
    Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId, string baseServiceUrl);
}