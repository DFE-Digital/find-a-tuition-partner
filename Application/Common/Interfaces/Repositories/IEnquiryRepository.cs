using Application.Common.Models.Enquiry.Manage;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IEnquiryRepository : IGenericRepository<Enquiry>
{
    Task<Enquiry?> GetEnquiryBySupportReferenceNumber(string supportReferenceNumber);
    Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(string supportReferenceNumber);
    Task<EnquiryResponse> GetEnquiryResponse(string supportReferenceNumber, string tuitionPartnerSeoUrl);
}