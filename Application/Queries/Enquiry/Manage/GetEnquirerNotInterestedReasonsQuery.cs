using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Manage;

namespace Application.Queries.Enquiry.Manage;

public record GetEnquirerNotInterestedReasonsQuery() : IRequest<List<EnquirerNotInterestedReasonModel>>;

public class GetEnquirerNotInterestedReasonsQueryHandler : IRequestHandler<GetEnquirerNotInterestedReasonsQuery, List<EnquirerNotInterestedReasonModel>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnquirerNotInterestedReasonsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EnquirerNotInterestedReasonModel>> Handle(GetEnquirerNotInterestedReasonsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EnquirerNotInterestedReasonRepository
            .GetEnquirerNotInterestedReasons();
    }
}