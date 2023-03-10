using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Application.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;
using TuitionType = Domain.Enums.TuitionType;

namespace Infrastructure.Repositories;

public class EnquiryRepository : GenericRepository<Enquiry>, IEnquiryRepository
{
    public EnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(int enquiryId, string baseServiceUrl)
    {
        var enquiry = await _context.Enquiries.AsNoTracking()
            .Where(e => e.Id == enquiryId)
            .Include(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(ks => ks.KeyStage)
            .Include(e => e.KeyStageSubjectEnquiry)
            .ThenInclude(s => s.Subject)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.EnquiryResponse)
            .ThenInclude(e => e!.MagicLink)
            .Include(x => x.TuitionPartnerEnquiry)
            .ThenInclude(x => x.TuitionPartner)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (enquiry == null)
        {
            return new EnquirerViewAllResponsesModel();
        }

        var tuitionPartnerEnquiriesWithResponses = enquiry.TuitionPartnerEnquiry.Where(x =>
            x.EnquiryResponse != null && !string.IsNullOrEmpty(x.EnquiryResponse.EnquiryResponseText)).ToList();

        var keyStageSubjects = enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();

        var tuitionTypeName = GetTuitionTypeName(enquiry.TuitionTypeId);

        var result = new EnquirerViewAllResponsesModel
        {
            EnquiryText = enquiry.EnquiryText!,
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            NumberOfTpEnquiryWasSent = enquiry.TuitionPartnerEnquiry.Count,
            KeyStageSubjects = keyStageSubjects,
            TuitionTypeName = tuitionTypeName,
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
                    $"{baseServiceUrl}/enquiry/respond/enquirer-response?token={er.EnquiryResponse!.MagicLink!.Token}"
            };
            result.EnquirerViewResponses.Add(responseModel);
        }

        var orderByReceivedEnquirerViewResponses = result.EnquirerViewResponses
            .OrderByDescending(x => x.EnquiryResponseDate).ToList();
        result.EnquirerViewResponses = orderByReceivedEnquirerViewResponses;
        return result;
    }

    private string GetTuitionTypeName(int? tuitionTypeId)
    {
        return tuitionTypeId switch
        {
            null => TuitionType.Any.ToString(),
            (int)TuitionType.Online => TuitionType.Online.ToString(),
            (int)TuitionType.InSchool => TuitionType.InSchool.ToString(),
            _ => TuitionType.Any.ToString()
        };
    }
}