using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Domain;
using Microsoft.EntityFrameworkCore;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Infrastructure.Repositories;

public class TuitionPartnerEnquiryRepository : GenericRepository<TuitionPartnerEnquiry>, ITuitionPartnerEnquiryRepository
{
    public TuitionPartnerEnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(int enquiryId, int tuitionPartnerId)
    {
        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.EnquiryId == enquiryId && e.TuitionPartnerId == tuitionPartnerId)
            .Include(e => e.Enquiry)
            .ThenInclude(m => m.MagicLinks)
            .Include(e => e.EnquiryResponse)
            .Include(x => x.TuitionPartner)
            .SingleOrDefaultAsync();

        if (tuitionPartnerEnquiry == null) return null;


        var enquirerViewAllResponsesMagicLinkToken = tuitionPartnerEnquiry.Enquiry.MagicLinks
            .SingleOrDefault(x => x.EnquiryId == enquiryId
                                       && x.MagicLinkTypeId == (int)MagicLinkType.EnquirerViewAllResponses);

        var result = new EnquirerViewResponseModel()
        {
            TuitionPartnerName = tuitionPartnerEnquiry.TuitionPartner.Name,
            EnquiryResponseText = tuitionPartnerEnquiry.EnquiryResponse!.TutoringLogisticsText,
            EnquirerViewAllResponsesToken = enquirerViewAllResponsesMagicLinkToken!.Token
        };

        return result;
    }
}