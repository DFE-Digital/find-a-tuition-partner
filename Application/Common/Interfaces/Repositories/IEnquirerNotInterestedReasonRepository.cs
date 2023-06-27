using Application.Common.Models.Enquiry.Manage;
using Domain;

namespace Application.Common.Interfaces.Repositories;

public interface IEnquirerNotInterestedReasonRepository : IGenericRepository<EnquirerNotInterestedReason>
{
    Task<List<EnquirerNotInterestedReasonModel>> GetEnquirerNotInterestedReasons();
}
