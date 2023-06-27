using Application.Common.Interfaces;
using Application.Extensions;
using Domain;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Enquiry.Build;

public record UpdateNotInterestedReasonCommand(
    string SupportReferenceNumber,
    string TuitionPartnerSeoUrl,
    int EnquirerNotInterestedReasonId,
    string EnquirerNotInterestedReason,
    string? EnquirerNotInterestedReasonAdditionalInfo) : IRequest<bool>
{ }

public class UpdateNotInterestedReasonCommandHandler : IRequestHandler<UpdateNotInterestedReasonCommand, bool>
{
    private const string NotInterestedOutcomeFeedbackKey = "outcome_feedback";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateNotInterestedReasonCommandHandler> _logger;

    private int? _tpEnquiryResponseId;

    public UpdateNotInterestedReasonCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateNotInterestedReasonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateNotInterestedReasonCommand request, CancellationToken cancellationToken)
    {
        var tpEnquiryResponse = await _unitOfWork.EnquiryRepository.GetEnquiryResponse(request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
        _tpEnquiryResponseId = tpEnquiryResponse.Id;

        var enquiryResponse = await UpdateEnquiryResponseWithNotInterestedFeedbackEmail(request);

        await _unitOfWork.Complete();

        _logger.LogInformation("Not interested feedback saved.  Email id: {emailId}, SupportReferenceNumber: {supportReferenceNumber}, TuitionPartnerSeoUrl: {tuitionPartnerSeoUrl}",
            enquiryResponse.TuitionPartnerResponseNotInterestedEmailLogId, request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        return true;
    }

    private async Task<EnquiryResponse> UpdateEnquiryResponseWithNotInterestedFeedbackEmail(UpdateNotInterestedReasonCommand request)
    {
        var enquirerNotInterestedReason = request.EnquirerNotInterestedReason;

        var enquiryResponse = await _unitOfWork.EnquiryResponseRepository
            .SingleOrDefaultAsync(x => x.Id == _tpEnquiryResponseId, "TuitionPartnerResponseNotInterestedEmailLog,TuitionPartnerResponseNotInterestedEmailLog.EmailPersonalisationLogs");

        if (enquiryResponse!.TuitionPartnerResponseNotInterestedEmailLog == null)
            throw new ArgumentException($"UpdateEnquiryResponseWithNotInterestedFeedbackEmail- No TuitionPartnerResponseNotInterestedEmailLog found for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        if (enquiryResponse!.TuitionPartnerResponseNotInterestedEmailLog.EmailPersonalisationLogs == null)
            throw new ArgumentException($"UpdateEnquiryResponseWithNotInterestedFeedbackEmail- No EmailPersonalisationLogs found for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        if (enquiryResponse!.TuitionPartnerResponseNotInterestedEmailLog.ProcessFromDate < DateTime.UtcNow)
            throw new ArgumentException($"UpdateEnquiryResponseWithNotInterestedFeedbackEmail- Already processed TuitionPartnerResponseNotInterestedEmailLog found for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        if (enquiryResponse!.EnquirerNotInterestedReasonId != null)
            throw new ArgumentException($"UpdateEnquiryResponseWithNotInterestedFeedbackEmail- EnquirerNotInterestedReason previously set for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        enquiryResponse!.EnquirerNotInterestedReasonId = request.EnquirerNotInterestedReasonId;

        if (!string.IsNullOrWhiteSpace(request.EnquirerNotInterestedReasonAdditionalInfo))
        {
            enquiryResponse!.EnquirerNotInterestedReasonAdditionalInfo = request.EnquirerNotInterestedReasonAdditionalInfo;
            enquirerNotInterestedReason = request.EnquirerNotInterestedReasonAdditionalInfo!;
        }

        var notInterestedOutcomeFeedbackPersonalisation = enquiryResponse!.TuitionPartnerResponseNotInterestedEmailLog!.EmailPersonalisationLogs.Single(x => x.Key == NotInterestedOutcomeFeedbackKey);

        enquirerNotInterestedReason = "The school’s feedback to your response:" +
                                                                Environment.NewLine +
                                                                Environment.NewLine +
                                                                enquirerNotInterestedReason;

        notInterestedOutcomeFeedbackPersonalisation.Value = "^ " + enquirerNotInterestedReason.EscapeNotifyText(true)!;

        return enquiryResponse;
    }
}
