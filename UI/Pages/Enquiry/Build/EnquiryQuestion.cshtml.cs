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

        var sessionId = Request.Cookies[StringConstants.SessionCookieName];

        if (sessionId == null) return RedirectToPage(nameof(EnquirerEmail));

        var sessionValues = await _sessionService.RetrieveDataAsync(sessionId);

        if (sessionValues == null) return Page();

        foreach (var sessionValue in sessionValues.Where(sessionValue => sessionValue.Key.Contains(StringConstants.EnquiryText)))
        {
            Data.EnquiryText = sessionValue.Value;
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(EnquiryQuestionModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        if (ModelState.IsValid)
        {
            var sessionId = Request.Cookies[StringConstants.SessionCookieName];

            if (sessionId != null)
            {
                await _sessionService.AddOrUpdateDataAsync(sessionId, new Dictionary<string, string>()
                {
                    { StringConstants.EnquiryText, data.EnquiryText!}
                });
            }

            return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
        }

        return Page();
    }
}