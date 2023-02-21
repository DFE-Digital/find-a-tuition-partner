using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;

namespace Application.Commands;

public record AddEnquiryResponseCommand : IRequest<bool>
{
    public EnquiryResponseModel Data { get; set; } = null!;
}

public class AddEnquiryResponseCommandHandler : IRequestHandler<AddEnquiryResponseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEnquiryResponseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddEnquiryResponseCommand request, CancellationToken cancellationToken)
    {
        var enquiryId = request.Data.EnquiryId;

        var tuitionPartnerId = request.Data.TuitionPartnerId;

        if (string.IsNullOrEmpty(request.Data.Token))
        {
            return false;
        }

        var magicLink =
            await _unitOfWork.MagicLinkRepository
                .SingleOrDefaultAsync(x => x.Token == request.Data.Token, null, true, cancellationToken);

        if (enquiryId == default || tuitionPartnerId == default) return false;

        var tpEnquiry = _unitOfWork.TuitionPartnerEnquiryRepository
            .Find(x => x.EnquiryId == enquiryId && x.TuitionPartnerId == tuitionPartnerId).SingleOrDefault();

        if (tpEnquiry == null) return false;

        tpEnquiry.EnquiryResponse = new EnquiryResponse()
        {
            EnquiryResponseText = request.Data?.EnquiryResponseText!,
            EnquiryId = enquiryId,
            MagicLinkId = magicLink?.Id
        };

        tpEnquiry.MagicLinkId = magicLink?.Id;

        return await _unitOfWork.Complete();

    }
}