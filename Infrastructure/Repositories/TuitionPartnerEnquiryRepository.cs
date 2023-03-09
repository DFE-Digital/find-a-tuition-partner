using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Application.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;
using MagicLinkType = Domain.Enums.MagicLinkType;

namespace Infrastructure.Repositories;

public class TuitionPartnerEnquiryRepository : GenericRepository<TuitionPartnerEnquiry>, ITuitionPartnerEnquiryRepository
{
    public TuitionPartnerEnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId, string baseServiceUrl)
    {
        var tuitionPartnerEnquiries = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.EnquiryId == enquiryId)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(ks => ks.KeyStage)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(s => s.Subject)
            .Include(e => e.EnquiryResponse)
            .ThenInclude(e => e!.MagicLink)
            .Include(x => x.TuitionPartner)
            .ToListAsync();

        if (!tuitionPartnerEnquiries.Any())
        {
            return new EnquirerViewAllResponsesModel();
        }

        var tuitionPartnerEnquiriesWithResponses = tuitionPartnerEnquiries.Where(x =>
            x.EnquiryResponse != null && !string.IsNullOrEmpty(x.EnquiryResponse.EnquiryResponseText)).ToList();

        var keyStageSubjects = tuitionPartnerEnquiries.First().Enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();

        var result = new EnquirerViewAllResponsesModel
        {
            EnquiryText = tuitionPartnerEnquiries.FirstOrDefault()?.Enquiry.EnquiryText!,
            SupportReferenceNumber = tuitionPartnerEnquiries.First().Enquiry.SupportReferenceNumber,
            NumberOfTpEnquiryWasSent = tuitionPartnerEnquiries.Count,
            KeyStageSubjects = keyStageSubjects,
            EnquiryCreatedDateTime = tuitionPartnerEnquiries.First().Enquiry.CreatedAt,
            EnquirerViewResponses = new List<EnquirerViewResponseDto>()
        };

        foreach (var er in tuitionPartnerEnquiriesWithResponses)
        {
            var responseModel = new EnquirerViewResponseDto
            {
                TuitionPartnerName = er.TuitionPartner.Name,
                EnquiryResponseDate = er.EnquiryResponse?.CreatedAt!,
                EnquirerEnquiryResponseLink =
                    $"{baseServiceUrl}/enquiry/respond/enquirer-response?token={er.EnquiryResponse!.MagicLink!.Token}"
            };
            result.EnquirerViewResponses.Add(responseModel);
        }

        var orderByReceivedEnquirerViewResponses = result.EnquirerViewResponses
            .OrderByDescending(x => x.EnquiryResponseDate).ToList();
        result.EnquirerViewResponses = orderByReceivedEnquirerViewResponses;
        return result;
    }

    public async Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(int enquiryId, int tuitionPartnerId)
    {
        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.EnquiryId == enquiryId && e.TuitionPartnerId == tuitionPartnerId)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e.MagicLinks)
            .Include(e => e.EnquiryResponse)
            .Include(x => x.TuitionPartner).SingleOrDefaultAsync();

        if (tuitionPartnerEnquiry == null) return null;


        var enquirerViewAllResponsesMagicLinkToken = tuitionPartnerEnquiry.Enquiry.MagicLinks
            .SingleOrDefault(x => x.MagicLinkTypeId == (int)MagicLinkType.EnquirerViewAllResponses);

        var result = new EnquirerViewResponseModel()
        {
            TuitionPartnerName = tuitionPartnerEnquiry.TuitionPartner.Name,
            EnquiryResponseText = tuitionPartnerEnquiry.EnquiryResponse!.EnquiryResponseText,
            EnquirerViewAllResponsesToken = enquirerViewAllResponsesMagicLinkToken!.Token
        };

        return result;
    }
}