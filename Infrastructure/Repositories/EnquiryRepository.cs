using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry;
using Application.Common.Models.Enquiry.Manage;
using Application.Extensions;
using Domain;
using Infrastructure.Extensions;
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
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.TuitionPartner)
            .SingleOrDefaultAsync();

        return enquiry ?? null;
    }

    public async Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(string supportReferenceNumber)
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
            .Include(x => x.TuitionSettings)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (enquiry == null)
        {
            throw new ArgumentException($"No enquiry found for enquiry ref {supportReferenceNumber}");
        }

        var tuitionPartnerEnquiriesWithResponses = enquiry.TuitionPartnerEnquiry.Where(x =>
            x.EnquiryResponse != null).ToList();

        var numberOfTpsDeclinedEnquiry = enquiry.TuitionPartnerEnquiry.Count(x => x.TuitionPartnerDeclinedEnquiry);

        var keyStageSubjects = enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();

        var result = new EnquirerViewAllResponsesModel
        {
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!,
            TutoringLogisticsDisplayModel = new TutoringLogisticsDisplayModel()
            {
                TutoringLogistics = enquiry.TutoringLogistics,
                TutoringLogisticsDetailsModel = enquiry.TutoringLogistics.ToTutoringLogisticsDetailsModel()
            },
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            NumberOfTpEnquiryWasSent = enquiry.TuitionPartnerEnquiry.Count,
            NumberOfTpsDeclinedEnquiry = numberOfTpsDeclinedEnquiry,
            KeyStageSubjects = keyStageSubjects,
            TuitionSettingName = enquiry.TuitionSettings.GetTuitionSettingName(),
            SENDRequirements = enquiry.SENDRequirements,
            AdditionalInformation = enquiry.AdditionalInformation,
            EnquiryCreatedDateTime = enquiry.CreatedAt.ToLocalDateTime(),
            EnquirerViewResponses = new List<EnquirerViewResponseDto>()
        };

        foreach (var er in tuitionPartnerEnquiriesWithResponses)
        {
            var responseModel = new EnquirerViewResponseDto
            {
                TuitionPartnerName = er.TuitionPartner.Name,
                EnquiryResponseDate = er.EnquiryResponse?.CreatedAt!
            };
            result.EnquirerViewResponses.Add(responseModel);
        }

        var orderByReceivedEnquirerViewResponses = result.EnquirerViewResponses
            .OrderByDescending(x => x.EnquiryResponseDate).ToList();
        result.EnquirerViewResponses = orderByReceivedEnquirerViewResponses;
        return result;
    }
}