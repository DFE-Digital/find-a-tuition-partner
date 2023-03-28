using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Application.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnquiryRepository : GenericRepository<Enquiry>, IEnquiryRepository
{
    public EnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<Enquiry?> GetEnquiryBySupportReferenceNumber(string supportReferenceNumber)
    {
        var enquiry = await _context.Enquiries.AsNoTracking()
            .Where(e => e.SupportReferenceNumber == supportReferenceNumber)
            .Include(x => x.MagicLink)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.MagicLink)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.EnquiryResponse)
            .SingleOrDefaultAsync();

        return enquiry ?? null;
    }

    public async Task<EnquirerViewAllResponsesModel?> GetEnquirerViewAllResponses(string baseServiceUrl, string supportReferenceNumber)
    {
        var enquiry = await _context.Enquiries.AsNoTracking()
            .Where(e => e.SupportReferenceNumber == supportReferenceNumber)
            .Include(e => e.MagicLink)
            .Include(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(ks => ks.KeyStage)
            .Include(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(s => s.Subject)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.EnquiryResponse)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.MagicLink)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.TuitionPartner)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (enquiry == null)
        {
            return null;
        }

        var tuitionPartnerEnquiriesWithResponses = enquiry.TuitionPartnerEnquiry.Where(x =>
            x.EnquiryResponse != null).ToList();

        var keyStageSubjects = enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();



        var result = new EnquirerViewAllResponsesModel
        {
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!,
            TutoringLogistics = enquiry.TutoringLogistics!,
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            NumberOfTpEnquiryWasSent = enquiry.TuitionPartnerEnquiry.Count,
            KeyStageSubjects = keyStageSubjects,
            TuitionTypeName = enquiry.TuitionTypeId.GetTuitionTypeName(),
            SENDRequirements = enquiry.SENDRequirements,
            AdditionalInformation = enquiry.AdditionalInformation,
            EnquiryCreatedDateTime = enquiry.CreatedAt,
            EnquirerViewResponses = new List<EnquirerViewResponseDto>()
        };

        foreach (var er in tuitionPartnerEnquiriesWithResponses)
        {
            var responseModel = new EnquirerViewResponseDto
            {
                TuitionPartnerName = er.TuitionPartner.Name,
                EnquiryResponseDate = er.EnquiryResponse?.CreatedAt!,
                EnquirerEnquiryResponseLink =
                    $"{baseServiceUrl}/enquiry/{supportReferenceNumber}/{er.TuitionPartner.Name.ToSeoUrl()}?Token={er.MagicLink!.Token}"
            };
            result.EnquirerViewResponses.Add(responseModel);
        }

        var orderByReceivedEnquirerViewResponses = result.EnquirerViewResponses
            .OrderByDescending(x => x.EnquiryResponseDate).ToList();
        result.EnquirerViewResponses = orderByReceivedEnquirerViewResponses;
        return result;
    }
}