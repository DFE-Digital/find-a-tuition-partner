using Application.Common.Models;

namespace UI.Pages;

public class Enquiry : PageModel
{
    private readonly IMediator _mediator;

    public Enquiry(IMediator mediator) => _mediator = mediator;
    [BindProperty] public EnquiryModel Data { get; set; } = new();

    [TempData] public string Message { get; set; } = null!;

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var shortListedTpsQuery = new GetAllShortlistedTuitionPartnersQuery();

        var shortListedTps = await _mediator.Send(shortListedTpsQuery);

        if (shortListedTps.Any())
        {
            Data.SelectedTuitionPartners = shortListedTps.ToList();
        }
        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        var dataSaved = await _mediator.Send(command);

        if (dataSaved)
        {
            Message = $"Enquiry successfully sent to the shortlisted tuition partners. You will receive a confirmation email shortly to the following email: {Data.Email}";
            ModelState.Clear();
            Data = new();
        }

        return Page();
    }
}