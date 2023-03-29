using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class AdditionalInformation : PageModel
{
    private readonly ISessionService _sessionService;

    public AdditionalInformation(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty] public AdditionalInformationModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(AdditionalInformationModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.AdditionalInformation = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryAdditionalInformation);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(AdditionalInformationModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryAdditionalInformation,
                string.IsNullOrWhiteSpace(data.AdditionalInformation) ? string.Empty : data.AdditionalInformation);

            return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
        }

        return Page();
    }
}