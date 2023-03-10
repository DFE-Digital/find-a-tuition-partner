using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class SendRequirements : PageModel
{
    private readonly ISessionService _sessionService;

    public SendRequirements(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty] public SendRequirementsModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(SendRequirementsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.SendRequirements = await _sessionService.RetrieveDataAsync(StringConstants.EnquirySendRequirements);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(SendRequirementsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirySendRequirements,
                string.IsNullOrWhiteSpace(data.SendRequirements) ? string.Empty : data.SendRequirements);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(AdditionalInformation), new SearchModel(data));
        }

        return Page();
    }
}