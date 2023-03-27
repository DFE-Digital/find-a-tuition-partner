using Application.Common.Models.Enquiry.Respond;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IEnquiryRepository : IGenericRepository<Enquiry>
{
    Task<Enquiry?> GetEnquiryBySupportReferenceNumber(string supportReferenceNumber);
    Task<EnquirerViewAllResponsesModel?> GetEnquirerViewAllResponses(string baseServiceUrl, string supportReferenceNumber);
}