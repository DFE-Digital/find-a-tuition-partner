using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry;
using Application.Common.Models.Enquiry.Manage;
using Application.Extensions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TuitionPartnerEnquiryRepository : GenericRepository<TuitionPartnerEnquiry>, ITuitionPartnerEnquiryRepository
{
    public TuitionPartnerEnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<EnquirerViewResponseModel?> GetEnquirerViewResponse(string supportReferenceNumber, string tuitionPartnerSeoUrl)
    {
        if (string.IsNullOrWhiteSpace(supportReferenceNumber) || string.IsNullOrWhiteSpace(tuitionPartnerSeoUrl)) return null;

        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Include(e => e.Enquiry)
            .Include(e => e.EnquiryResponse)
            .Include(x => x.TuitionPartner)
            .Include(e => e.Enquiry.KeyStageSubjectEnquiry)
            .ThenInclude(ks => ks.KeyStage)
            .Include(e => e.Enquiry.KeyStageSubjectEnquiry)
            .ThenInclude(s => s.Subject)
            .AsSplitQuery()
            .SingleOrDefaultAsync(e => e.Enquiry.SupportReferenceNumber == supportReferenceNumber
                                  && e.TuitionPartner.SeoUrl == tuitionPartnerSeoUrl);

        if (tuitionPartnerEnquiry == null) return null;

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
            EnquiryTutoringLogisticsDisplayModel = new TutoringLogisticsDisplayModel()
            {
                TutoringLogistics = enquiry.TutoringLogistics,
                TutoringLogisticsDetailsModel = enquiry.TutoringLogistics.ToTutoringLogisticsDetailsModel()
            },
            EnquirySENDRequirements = enquiry.SENDRequirements,
            EnquiryAdditionalInformation = enquiry.AdditionalInformation,
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

    public async Task<EnquirerViewTuitionPartnerDetailsModel?> GetEnquirerViewTuitionPartnerDetailsResponse(string supportReferenceNumber, string tuitionPartnerSeoUrl)
    {
        if (string.IsNullOrWhiteSpace(supportReferenceNumber) || string.IsNullOrWhiteSpace(tuitionPartnerSeoUrl)) return null;

        var tuitionPartnerEnquiry = await _context.TuitionPartnersEnquiry.AsNoTracking()
            .Where(e => e.Enquiry.SupportReferenceNumber == supportReferenceNumber
                        && e.TuitionPartner.SeoUrl == tuitionPartnerSeoUrl)
            .Include(e => e.Enquiry)
            .Include(x => x.TuitionPartner)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (tuitionPartnerEnquiry == null) return null;

        var enquiry = tuitionPartnerEnquiry.Enquiry;
        var enquiryTP = tuitionPartnerEnquiry.TuitionPartner;

        var result = new EnquirerViewTuitionPartnerDetailsModel
        {
            TuitionPartnerName = enquiryTP.Name,
            TuitionPartnerPhoneNumber = enquiryTP.PhoneNumber,
            TuitionPartnerEmailAddress = enquiryTP.Email,
            TuitionPartnerWebsite = enquiryTP.Website,
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!
        };

        return result;
    }
}