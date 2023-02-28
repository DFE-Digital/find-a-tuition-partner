using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Application.Constants;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionPartnerEnquiryRepository : GenericRepository<TuitionPartnerEnquiry>, ITuitionPartnerEnquiryRepository
{
    public TuitionPartnerEnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId)
    {
        var tuitionPartnerEnquiries = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.EnquiryId == enquiryId)
            .Include(e => e.Enquiry)
            .Include(e => e.EnquiryResponse)
            .Include(x => x.TuitionPartner).ToListAsync();

        if (!tuitionPartnerEnquiries.Any())
        {
            return new EnquirerViewAllResponsesModel();
        }

        var result = new EnquirerViewAllResponsesModel
        {
            EnquiryText = tuitionPartnerEnquiries.FirstOrDefault()?.Enquiry.EnquiryText!,
            EnquirerViewResponses = new List<EnquirerViewResponseModel>()
        };

        foreach (var responseModel in tuitionPartnerEnquiries.Select(er => new EnquirerViewResponseModel
        {
            TuitionPartnerName = er.TuitionPartner.Name,
            EnquiryResponse = er.EnquiryResponse?.EnquiryResponseText!,
            Status = string.IsNullOrEmpty(er.EnquiryResponse?.EnquiryResponseText!) ? StringConstants.PENDING : StringConstants.RECEIVED
        }))
        {
            result.EnquirerViewResponses.Add(responseModel);
        }

        var orderByReceivedEnquirerViewResponses = result.EnquirerViewResponses.OrderByDescending(x => x.Status == StringConstants.RECEIVED).ToList();
        result.EnquirerViewResponses = orderByReceivedEnquirerViewResponses;
        return result;
    }
}