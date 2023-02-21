using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Queries;

public record GetEnquirerViewAllResponsesQuery(int EnquiryId) : IRequest<EnquirerViewAllResponsesModel>;

public class GetEnquirerViewAllResponsesQueryHandler : IRequestHandler<GetEnquirerViewAllResponsesQuery, EnquirerViewAllResponsesModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnquirerViewAllResponsesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnquirerViewAllResponsesModel> Handle(GetEnquirerViewAllResponsesQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TuitionPartnerEnquiryRepository
            .GetEnquirerViewAllResponses(request.EnquiryId);
        return result;
    }
}