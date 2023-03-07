using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class EnquiryQuestion : PageModel
{
    private readonly ISessionService _sessionService;

    public EnquiryQuestion(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty(SupportsGet = true)] public EnquiryQuestionModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(EnquiryQuestionModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.EnquiryText = await _sessionService.RetrieveDataAsync(StringConstants.EnquiryText);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(EnquiryQuestionModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquiryText, data.EnquiryText!);

            return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
        }

        return Page();
    }
}