using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Manage;
using Application.Extensions;
using Domain;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

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
            .Include(s => s.Enquiry)
            .ThenInclude(s => s.TuitionSettings)
            .AsSplitQuery()
            .SingleOrDefaultAsync(e => e.Enquiry.SupportReferenceNumber == supportReferenceNumber
                                  && e.TuitionPartner.SeoUrl == tuitionPartnerSeoUrl);

        if (tuitionPartnerEnquiry == null ||
            tuitionPartnerEnquiry.Enquiry == null ||
            tuitionPartnerEnquiry.EnquiryResponse == null) return null;

        var enquiry = tuitionPartnerEnquiry.Enquiry;
        var enquiryResponse = tuitionPartnerEnquiry.EnquiryResponse;

        var keyStageSubjects = enquiry
            .KeyStageSubjectEnquiry
            .Select(x => $"{x.KeyStage.Name}: {x.Subject.Name}")
            .GroupByKeyAndConcatenateValues();

        return new EnquirerViewResponseModel
        {
            TuitionPartnerName = tuitionPartnerEnquiry.TuitionPartner.Name,
            EnquiryKeyStageSubjects = keyStageSubjects,
            EnquiryTuitionSetting = enquiry.TuitionSettings.GetTuitionSettingName(),
            EnquiryTutoringLogistics = enquiry.TutoringLogistics,
            EnquirySENDRequirements = enquiry.SENDRequirements,
            EnquiryAdditionalInformation = enquiry.AdditionalInformation,
            LocalAuthorityDistrict = enquiry.LocalAuthorityDistrict!,
            SupportReferenceNumber = enquiry.SupportReferenceNumber,
            KeyStageAndSubjectsText = enquiryResponse.KeyStageAndSubjectsText,
            TuitionSettingText = enquiryResponse.TuitionSettingText,
            TutoringLogisticsText = enquiryResponse.TutoringLogisticsText,
            SENDRequirementsText = enquiryResponse.SENDRequirementsText,
            AdditionalInformationText = enquiryResponse.AdditionalInformationText,
            EnquiryResponseStatus = (EnquiryResponseStatus)enquiryResponse.EnquiryResponseStatusId
        };
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