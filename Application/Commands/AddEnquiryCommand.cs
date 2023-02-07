using Application.Common.Models;
using Application.Repositories;
using Domain;

namespace Application.Commands;

public record AddEnquiryCommand : IRequest<bool>
{
    public EnquiryModel? Data { get; set; } = null!;
}

public class AddEnquiryCommandHandler : IRequestHandler<AddEnquiryCommand, bool>
{
    private readonly IEnquiryRepository _enquiryRepository;

    public AddEnquiryCommandHandler(IEnquiryRepository enquiryRepository)
    {
        _enquiryRepository = enquiryRepository;
    }

    public async Task<bool> Handle(AddEnquiryCommand request, CancellationToken cancellationToken)
    {
        var enquiry = new Enquiry()
        {
            EnquiryText = request.Data?.EnquiryText!,
            Email = request.Data?.Email!,
            TuitionPartners = request.Data?.SelectedTuitionPartners!
        };
        return await _enquiryRepository.AddEnquiryAsync(enquiry, cancellationToken);
    }
}