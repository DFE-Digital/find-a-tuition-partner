using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace Application.Queries;

public record GetEnquirerViewAllResponsesQuery(int EnquiryId, string BaseServiceUrl) : IRequest<EnquirerViewAllResponsesModel>;

public class GetEnquirerViewAllResponsesQueryHandler : IRequestHandler<GetEnquirerViewAllResponsesQuery, EnquirerViewAllResponsesModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnquirerViewAllResponsesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnquirerViewAllResponsesModel> Handle(GetEnquirerViewAllResponsesQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.EnquiryRepository
            .GetEnquirerViewAllResponses(request.EnquiryId, request.BaseServiceUrl);
        return result;
    }
}