using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Manage;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Enquiry.Manage;

public record GetEnquirerViewResponseQuery(string SupportReferenceNumber, string TuitionPartnerSeoUrl) : IRequest<EnquirerViewResponseModel?>;

public class GetEnquirerViewResponseQueryHandler : IRequestHandler<GetEnquirerViewResponseQuery, EnquirerViewResponseModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetEnquirerViewResponseQueryHandler> _logger;

    public GetEnquirerViewResponseQueryHandler(IUnitOfWork unitOfWork, ILogger<GetEnquirerViewResponseQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnquirerViewResponseModel?> Handle(GetEnquirerViewResponseQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewResponse(request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        if (result == null)
        {
            _logger.LogInformation("Enquiry response not found for the given SupportReferenceNumber: {supportReferenceNumber} and TuitionPartnerSeoUrl: {tuitionPartnerSeoUrl}",
                request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
        }
        else if (result.EnquiryResponseStatus == Domain.Enums.EnquiryResponseStatus.NotInterested)
        {
            _logger.LogInformation("Enquiry response previously not interested for the given SupportReferenceNumber: {supportReferenceNumber} and TuitionPartnerSeoUrl: {tuitionPartnerSeoUrl}",
                request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
            result = null;
        }

        return result;
    }
}