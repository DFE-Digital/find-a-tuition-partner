using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class TutoringLogistics : PageModel
{
    private readonly ISessionService _sessionService;

    public TutoringLogistics(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty] public TutoringLogisticsModel Data { get; set; } = new();

    public int ContentCountRaw { get; set; } = 0;
    public int ContentCountReplaced { get; set; } = 0;

    public async Task<IActionResult> OnGetAsync(TutoringLogisticsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        ContentCountRaw = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Length;
        ContentCountReplaced = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace(Environment.NewLine, " ").Length;

        Data.TutoringLogistics = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryTutoringLogistics);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(TutoringLogisticsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        ContentCountRaw = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Length;
        ContentCountReplaced = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace(Environment.NewLine, " ").Length;


        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryTutoringLogistics, data.TutoringLogistics!);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(SENDRequirements), new SearchModel(data));
        }

        return Page();
    }
}