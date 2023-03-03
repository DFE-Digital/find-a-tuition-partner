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

    public async Task<IActionResult> OnGet()
    {
        var sessionValues = await _sessionService.RetrieveDataAsync();

        if (sessionValues != null)
        {
            foreach (var sessionValue in sessionValues.Where(sessionValue => sessionValue.Key.Contains(StringConstants.EnquiryText)))
            {
                Data.EnquiryText = sessionValue.Value;
            }
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnGetSubmit(EnquiryQuestionModel data)
    {
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
            {
                { StringConstants.EnquiryText, data.EnquiryText!}
            });

            return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
        }

        return Page();
    }
}