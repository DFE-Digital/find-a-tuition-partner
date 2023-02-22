using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Infrastructure.Repositories;

public class MagicLinkRepository : GenericRepository<MagicLink>, IMagicLinkRepository
{
    public MagicLinkRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerEnquiryResponseReceivedDto?> GetEnquirerEnquiryResponseReceivedData(int enquiryId, int tuitionPartnerId)
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
        return new EnquirerEnquiryResponseReceivedDto()
        {
            TuitionPartnerName = tpName,
            Email = magicLink.Enquiry.Email,
            EnquiryText = magicLink.Enquiry.EnquiryText,
            Token = magicLink.Token
        };
    }
}