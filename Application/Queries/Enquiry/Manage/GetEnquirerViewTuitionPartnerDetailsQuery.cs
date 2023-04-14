using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Manage;
using Microsoft.Extensions.Logging;

namespace Application.Queries.Enquiry.Manage;

public record GetEnquirerViewTuitionPartnerDetailsQuery(string SupportReferenceNumber, string TuitionPartnerSeoUrl) : IRequest<EnquirerViewTuitionPartnerDetailsModel?>;

public class GetEnquirerViewTuitionPartnerDetailsQueryHandler : IRequestHandler<GetEnquirerViewTuitionPartnerDetailsQuery, EnquirerViewTuitionPartnerDetailsModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<GetEnquirerViewTuitionPartnerDetailsQueryHandler> _logger;

    public GetEnquirerViewTuitionPartnerDetailsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetEnquirerViewTuitionPartnerDetailsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<EnquirerViewTuitionPartnerDetailsModel?> Handle(GetEnquirerViewTuitionPartnerDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewTuitionPartnerDetailsResponse(request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);

        if (result == null)
        {
            _logger.LogInformation("Enquiry response details not found for the given SupportReferenceNumber: {supportReferenceNumber} and TuitionPartnerSeoUrl: {tuitionPartnerSeoUrl}",
                request.SupportReferenceNumber, request.TuitionPartnerSeoUrl);
        }

        return result;
    }
}