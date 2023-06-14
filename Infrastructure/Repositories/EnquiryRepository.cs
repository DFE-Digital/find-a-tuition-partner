using Application.Common.DTO;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Enquiry.Manage;
using Application.Extensions;
using Domain;
using Domain.Enums;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;

namespace Infrastructure.Repositories;

public class EnquiryRepository : GenericRepository<Enquiry>, IEnquiryRepository
{
    public EnquiryRepository(NtpDbContext context) : base(context)
    {
    }

    public async Task<Enquiry?> GetEnquiryBySupportReferenceNumber(string supportReferenceNumber)
    {
        var enquiry = await _context.Enquiries
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

    public async Task<EnquirerViewAllResponsesModel> GetEnquirerViewAllResponses(string supportReferenceNumber, EnquirerResponseResultsModel enquirerResponseResultsModel)
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
            .ThenInclude(x => x!.EnquiryResponseStatus)
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
            TuitionSettingName = enquiry.TuitionSettings.GetTuitionSettingName(),
            SENDRequirements = enquiry.SENDRequirements,
            AdditionalInformation = enquiry.AdditionalInformation,
            EnquiryCreatedDateTime = enquiry.CreatedAt.ToLocalDateTime(),
            EnquirerViewResponses = new List<EnquirerViewResponseDto>(),
            EnquiryResponseResultsOrderBy = enquirerResponseResultsModel.EnquiryResponseResultsOrderBy,
            EnquiryResponseResultsDirection = enquirerResponseResultsModel.EnquiryResponseResultsDirection
        };

        foreach (var er in tuitionPartnerEnquiriesWithResponses)
        {
            var responseModel = new EnquirerViewResponseDto
            {
                TuitionPartnerName = er.TuitionPartner.Name,
                EnquiryResponseDate = er.EnquiryResponse!.CreatedAt!,
                EnquiryResponseStatus = (EnquiryResponseStatus)er.EnquiryResponse!.EnquiryResponseStatusId,
                EnquiryResponseStatusOrderBy = er.EnquiryResponse!.EnquiryResponseStatus.OrderBy
            };
            result.EnquirerViewResponses.Add(responseModel);
        }

        result.NumberOfEnquirerNotInterestedResponses = result.EnquirerViewResponses.Count(x => x.EnquiryResponseStatus == EnquiryResponseStatus.NotInterested);

        result.EnquirerViewResponses = result.EnquirerViewResponses
            .Where(x => x.EnquiryResponseStatus != EnquiryResponseStatus.NotInterested).ToList();

        result.EnquirerViewResponses = OrderEnquirerResponseResults(result.EnquirerViewResponses, enquirerResponseResultsModel);

        return result;
    }

    public async Task<EnquiryResponse> GetEnquiryResponse(string supportReferenceNumber, string tuitionPartnerSeoUrl)
    {
        if (string.IsNullOrWhiteSpace(supportReferenceNumber))
            throw new ArgumentException("SupportReferenceNumber is null");

        if (string.IsNullOrWhiteSpace(tuitionPartnerSeoUrl))
            throw new ArgumentException("TuitionPartnerSeoUrl is null");

        var enquiry = await GetEnquiryBySupportReferenceNumber(supportReferenceNumber) ??
            throw new ArgumentException($"No enquiry found for SupportReferenceNumber {supportReferenceNumber}");

        var tpEnquiry = enquiry.TuitionPartnerEnquiry
            .SingleOrDefault(x => x.TuitionPartner.SeoUrl == tuitionPartnerSeoUrl) ??
            throw new ArgumentException($"No TuitionPartnerEnquiry found for SupportReferenceNumber {supportReferenceNumber} and TP {tuitionPartnerSeoUrl}");

        var tpEnquiryResponse = tpEnquiry.EnquiryResponse ??
            throw new ArgumentException($"No EnquiryResponse found for SupportReferenceNumber {supportReferenceNumber} and TP {tuitionPartnerSeoUrl}");

        return tpEnquiryResponse!;
    }

    private static List<EnquirerViewResponseDto> OrderEnquirerResponseResults(List<EnquirerViewResponseDto> results,
        EnquirerResponseResultsModel enquirerResponseResultsModel)
    {
        if (!results.Any())
            return results;

        return enquirerResponseResultsModel.EnquiryResponseResultsOrderBy switch
        {
            EnquiryResponseResultsOrderBy.Date => enquirerResponseResultsModel.EnquiryResponseResultsDirection == OrderByDirection.Descending
                                ? results.OrderByDescending(e => e.EnquiryResponseDate).ToList()
                                : results.OrderBy(e => e.EnquiryResponseDate).ToList(),

            EnquiryResponseResultsOrderBy.Name => enquirerResponseResultsModel.EnquiryResponseResultsDirection == OrderByDirection.Descending
                                ? results.OrderByDescending(e => e.TuitionPartnerName).ToList()
                                : results.OrderBy(e => e.TuitionPartnerName).ToList(),

            _ => enquirerResponseResultsModel.EnquiryResponseResultsDirection == OrderByDirection.Descending
                                ? results
                                    .OrderByDescending(e => e.EnquiryResponseStatusOrderBy)
                                    .ThenByDescending(e => e.EnquiryResponseDate).ToList()
                                : results
                                    .OrderBy(e => e.EnquiryResponseStatusOrderBy)
                                    .ThenByDescending(e => e.EnquiryResponseDate).ToList(),
        };
    }
}