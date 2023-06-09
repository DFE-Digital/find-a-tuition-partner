using Application.Common.Interfaces;
using Application.Extensions;
using Microsoft.Extensions.Logging;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;
namespace Application.Commands.Enquiry.Manage;

public record UpdateEnquiryStatusCommand(string SupportReferenceNumber, string TuitionPartnerSeoUrl, EnquiryResponseStatus EnquiryResponseStatus) : IRequest<bool> { }

public class UpdateEnquiryStatusCommandHandler : IRequestHandler<UpdateEnquiryStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEnquiryStatusCommandHandler> _logger;

    public UpdateEnquiryStatusCommandHandler(IUnitOfWork unitOfWork,
        ILogger<UpdateEnquiryStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateEnquiryStatusCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _unitOfWork.EnquiryRepository.GetEnquiryBySupportReferenceNumber(request.SupportReferenceNumber) ??
            throw new ArgumentException($"No enquiry found in UpdateEnquiryStatusCommandHandler for SupportReferenceNumber {request.SupportReferenceNumber}");

        var tpEnquiry = enquiry.TuitionPartnerEnquiry
            .SingleOrDefault(x => x.TuitionPartner.SeoUrl == request.TuitionPartnerSeoUrl) ??
            throw new ArgumentException($"No TuitionPartnerEnquiry found in UpdateEnquiryStatusCommandHandler for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        var tpEnquiryResponse = tpEnquiry.EnquiryResponse ??
            throw new ArgumentException($"No EnquiryResponse found in UpdateEnquiryStatusCommandHandler for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        if (tpEnquiryResponse.EnquiryResponseStatusId == (int)EnquiryResponseStatus.Rejected)
            throw new ArgumentException($"EnquiryResponse already Rejected in UpdateEnquiryStatusCommandHandler for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        tpEnquiryResponse.EnquiryResponseStatusId = (int)request.EnquiryResponseStatus;

        await _unitOfWork.Complete();

        _logger.LogInformation("Updated status of enquiry to {newStatus}, support ref {supportRef}, TP {tp}", request.EnquiryResponseStatus.DisplayName(),
            request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        return true;
    }
}
