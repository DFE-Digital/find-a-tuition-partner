using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Respond;
using Application.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionPartnerEnquiryRepository : GenericRepository<TuitionPartnerEnquiry>, ITuitionPartnerEnquiryRepository
{
    public TuitionPartnerEnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(string supportReferenceNumber, string magicLinkToken)
    {
        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Include(e => e.MagicLink)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e.MagicLink)
            .Include(e => e.EnquiryResponse)
            .Include(x => x.TuitionPartner)
            .Include(e => e.Enquiry.KeyStageSubjectEnquiry)
            .ThenInclude(ks => ks.KeyStage)
            .Include(e => e.Enquiry.KeyStageSubjectEnquiry)
            .ThenInclude(s => s.Subject)
            .AsSplitQuery()
            .SingleOrDefaultAsync(e => e.Enquiry.SupportReferenceNumber == supportReferenceNumber
                                  && e.MagicLink!.Token == magicLinkToken);

        if (tuitionPartnerEnquiry == null) return null;


        var enquirerViewAllResponsesMagicLinkToken = tuitionPartnerEnquiry.Enquiry.MagicLink;

        var enquiry = tuitionPartnerEnquiry.Enquiry;

        var keyStageSubjects = enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();

        var result = new EnquirerViewResponseModel
        {
            TuitionPartnerName = tuitionPartnerEnquiry.TuitionPartner.Name,
            EnquiryKeyStageSubjects = keyStageSubjects,
            EnquiryTuitionType = enquiry.TuitionTypeId.GetTuitionTypeName(),
            EnquiryTutoringLogistics = enquiry.TutoringLogistics,
            EnquirySENDRequirements = enquiry.SENDRequirements,
            EnquiryAdditionalInformation = enquiry.AdditionalInformation,
            EnquirerViewAllResponsesToken = enquirerViewAllResponsesMagicLinkToken!.Token,
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!,
            SupportReferenceNumber = enquiry.SupportReferenceNumber
        };

        var enquiryResponse = tuitionPartnerEnquiry.EnquiryResponse;

        if (enquiryResponse != null)
        {
            result.KeyStageAndSubjectsText = enquiryResponse.KeyStageAndSubjectsText;
            result.TuitionTypeText = enquiryResponse.TuitionTypeText;
            result.TutoringLogisticsText = enquiryResponse.TutoringLogisticsText;
            result.SENDRequirementsText = enquiryResponse.SENDRequirementsText;
            result.AdditionalInformationText = enquiryResponse.AdditionalInformationText;
        }

        return result;
    }

    public async Task<EnquirerViewTuitionPartnerDetailsModel?> GetEnquirerViewTuitionPartnerDetailsResponse(string supportReferenceNumber,
        string magicLinkToken)
    {
        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.Enquiry.SupportReferenceNumber == supportReferenceNumber
                        && e.MagicLink!.Token == magicLinkToken)
            .Include(e => e.Enquiry)
            .ThenInclude(e => e.MagicLink)
            .Include(x => x.TuitionPartner)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (tuitionPartnerEnquiry == null) return null;

        var enquirerViewAllResponsesMagicLinkToken = tuitionPartnerEnquiry.Enquiry.MagicLink;

        var enquiry = tuitionPartnerEnquiry.Enquiry;
        var enquiryTP = tuitionPartnerEnquiry.TuitionPartner;

        var result = new EnquirerViewTuitionPartnerDetailsModel
        {
            TuitionPartnerName = enquiryTP.Name,
            TuitionPartnerPhoneNumber = enquiryTP.PhoneNumber,
            TuitionPartnerEmailAddress = enquiryTP.Email,
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            EnquirerViewAllResponsesToken = enquirerViewAllResponsesMagicLinkToken!.Token,
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!
        };

        return result;
    }
}