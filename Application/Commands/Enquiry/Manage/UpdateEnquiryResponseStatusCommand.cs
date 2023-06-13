using Application.Common.Interfaces;
using Application.Extensions;
using Microsoft.Extensions.Logging;
using EnquiryResponseStatus = Domain.Enums.EnquiryResponseStatus;
namespace Application.Commands.Enquiry.Manage;

public record UpdateEnquiryResponseStatusCommand(string SupportReferenceNumber, string TuitionPartnerSeoUrl, EnquiryResponseStatus EnquiryResponseStatus) : IRequest<bool> { }

public class UpdateEnquiryResponseStatusCommandHandler : IRequestHandler<UpdateEnquiryResponseStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateEnquiryResponseStatusCommandHandler> _logger;

    public UpdateEnquiryResponseStatusCommandHandler(IUnitOfWork unitOfWork,
        ILogger<UpdateEnquiryResponseStatusCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateEnquiryResponseStatusCommand request, CancellationToken cancellationToken)
    {
        var tpEnquiryResponse = await _unitOfWork.EnquiryRepository.GetEnquiryResponse(request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        if (tpEnquiryResponse.EnquiryResponseStatusId == (int)EnquiryResponseStatus.NotInterested)
            throw new ArgumentException($"EnquiryResponse already Not Interested in UpdateEnquiryResponseStatusCommandHandler for SupportReferenceNumber {request.SupportReferenceNumber} and TP {request.TuitionPartnerSeoUrl}");

        tpEnquiryResponse.EnquiryResponseStatusId = (int)request.EnquiryResponseStatus;

        await _unitOfWork.Complete();

        _logger.LogInformation("Updated status of enquiry to {newStatus}, support ref {supportRef}, TP {tp}", request.EnquiryResponseStatus.DisplayName(),
            request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        return true;
    }
}
