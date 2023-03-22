using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class SENDRequirements : PageModel
{
    private readonly ISessionService _sessionService;

    public SENDRequirements(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty] public SENDRequirementsModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(SENDRequirementsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.SENDRequirements = await _sessionService.RetrieveDataByKeyAsync(StringConstants.EnquirySENDRequirements);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(SENDRequirementsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirySENDRequirements,
                string.IsNullOrWhiteSpace(data.SENDRequirements) ? string.Empty : data.SENDRequirements);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(AdditionalInformation), new SearchModel(data));
        }

        return Page();
    }
}