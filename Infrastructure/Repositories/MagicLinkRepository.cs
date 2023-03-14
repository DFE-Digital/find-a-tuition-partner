using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Domain;
using Microsoft.EntityFrameworkCore;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Infrastructure.Repositories;

public class MagicLinkRepository : GenericRepository<MagicLink>, IMagicLinkRepository
{
    public MagicLinkRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquiryResponseModel?> GetEnquirerEnquiryResponseReceivedData(int enquiryId, int tuitionPartnerId)
    {
        var magicLink = await _context.MagicLinks.AsNoTracking()
            .Where(e => e.EnquiryId == enquiryId && e.MagicLinkTypeId == (int)MagicLinkType.EnquirerViewAllResponses)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e!.TuitionPartnerEnquiry)
            .ThenInclude(e => e!.TuitionPartner).SingleOrDefaultAsync();

        if (magicLink == null) return null;

        var tpName = string.Empty;

        if (magicLink.Enquiry!.TuitionPartnerEnquiry.Any())
        {
            tpName = magicLink.Enquiry.TuitionPartnerEnquiry
                .First(x => x.TuitionPartnerId == tuitionPartnerId).TuitionPartner.Name;
        }
        return new EnquiryResponseModel()
        {
            TuitionPartnerName = tpName,
            Email = magicLink.Enquiry.Email,
            TutoringLogisticsText = magicLink.Enquiry.TutoringLogistics,
            Token = magicLink.Token
        };
    }
}