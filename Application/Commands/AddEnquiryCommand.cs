using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<bool>
{
    public EnquiryModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddEnquiryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        var matchedSeoUrls = _unitOfWork.TuitionPartnerRepository.GetMatchedSeoUrls(request.Data!.SelectedTuitionPartners!).ToList();

        if (!matchedSeoUrls.Any()) return false;
        var tuitionPartnerEnquirySeoUrl = matchedSeoUrls.Select(selectedTuitionPartner =>
            new TuitionPartnerEnquirySeoUrl() { SeoUrl = selectedTuitionPartner }).ToList();

        var enquiry = new Enquiry()
        {
            EnquiryText = request.Data?.EnquiryText!,
            Email = request.Data?.Email!,
            TuitionPartnerEnquirySeoUrl = tuitionPartnerEnquirySeoUrl
        };

        _unitOfWork.EnquiryRepository.AddAsync(enquiry, cancellationToken);

        return await _unitOfWork.Complete();
    }
}