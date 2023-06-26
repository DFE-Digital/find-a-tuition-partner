using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Manage;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnquirerNotInterestedReasonRepository : GenericRepository<EnquirerNotInterestedReason>, IEnquirerNotInterestedReasonRepository
{
    public EnquirerNotInterestedReasonRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<List<EnquirerNotInterestedReasonModel>> GetEnquirerNotInterestedReasons()
    {
        return await _context.EnquirerNotInterestedReasons
            .Where(e => e.IsActive)
            .OrderBy(e => e.OrderBy)
            .Select(e => new EnquirerNotInterestedReasonModel()
            {
                Id = e.Id,
                Description = e.Description,
                CollectAdditionalInfoIfSelected = e.CollectAdditionalInfoIfSelected
            }).ToListAsync();
    }
}