using Application.Common.DTO;
using Application.Common.Interfaces;

namespace Application.Queries;

public record GetMagicLinkTokenQuery(string Token, string TokenType) : IRequest<MagicLinkDto?>;

public class GetMagicLinkTokenQueryHandler : IRequestHandler<GetMagicLinkTokenQuery, MagicLinkDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMagicLinkTokenQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MagicLinkDto?> Handle(GetMagicLinkTokenQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Token)) return null;

        var magicLink = await _unitOfWork.MagicLinkRepository.
            SingleOrDefaultAsync(x => x.Token == request.Token
                                      && x.MagicLinkType!.Name == request.TokenType,
                "MagicLinkType", false, cancellationToken);

        if (magicLink != null)
        {
            return new MagicLinkDto()
            {
                EnquiryId = magicLink.EnquiryId,
                ExpirationDate = magicLink.ExpirationDate,
                Token = magicLink.Token,
                MagicLinkTypeId = magicLink.MagicLinkTypeId
            };
        }

        return null;
    }
}