using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class EnquirerEmail : PageModel
{
    private readonly ISessionService _sessionService;

    public EnquirerEmail(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }
    [BindProperty] public EnquirerEmailModel Data { get; set; } = new();

    public async Task<IActionResult> OnGet(EnquirerEmailModel data)
    {
        Data = data;

        var sessionValues = await _sessionService.RetrieveDataAsync();

        if (sessionValues?.TryGetValue(StringConstants.EnquirerEmail, out var email) == true)
        {
            Data.Email = email;
        }

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnGetSubmit(EnquirerEmailModel data)
    {
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
            {
                { StringConstants.EnquirerEmail, data.Email!}

            });

            if (!string.IsNullOrEmpty(data.Postcode))
            {
                await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
                {
                    { StringConstants.PostCode, data.Postcode },

                });
            }

            if (data.KeyStages != null)
            {
                await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
                {
                    { StringConstants.KeyStages, string.Join(",", data.KeyStages) },
                    { StringConstants.Subjects, string.Join(",", data.Subjects!) },
                });
            }

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers));
            }

            return RedirectToPage(nameof(EnquiryQuestion), new SearchModel(data));
        }

        return Page();
    }
}