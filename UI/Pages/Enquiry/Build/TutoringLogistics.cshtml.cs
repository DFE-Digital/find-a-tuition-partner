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
    public int ContentCountReplacedWithEmpty { get; set; } = 0;
    public int ContentCountReplacedUsingStringR { get; set; } = 0;
    public int ContentCountReplacedUsingStringRWithEmpty { get; set; } = 0;
    public int ContentCountReplacedUsingStringN { get; set; } = 0;
    public int ContentCountReplacedUsingStringNWithEmpty { get; set; } = 0;
    public int ContentCountReplacedUsingStringRN { get; set; } = 0;
    public int ContentCountReplacedUsingStringRNWithEmpty { get; set; } = 0;
    public int ContentCountReplacedUsingStringRNWithN { get; set; } = 0;

    public async Task<IActionResult> OnGetAsync(TutoringLogisticsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        ContentCountRaw = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Length;
        ContentCountReplaced = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace(Environment.NewLine, " ").Length;
        ContentCountReplacedWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace(Environment.NewLine, "").Length;
        ContentCountReplacedUsingStringR = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r", " ").Length;
        ContentCountReplacedUsingStringRWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r", "").Length;
        ContentCountReplacedUsingStringN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\n", " ").Length;
        ContentCountReplacedUsingStringNWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\n", "").Length;
        ContentCountReplacedUsingStringRN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", " ").Length;
        ContentCountReplacedUsingStringRNWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", "").Length;
        ContentCountReplacedUsingStringRNWithN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", "\n").Length;

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
        ContentCountReplacedWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace(Environment.NewLine, "").Length;
        ContentCountReplacedUsingStringR = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r", " ").Length;
        ContentCountReplacedUsingStringRWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r", "").Length;
        ContentCountReplacedUsingStringN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\n", " ").Length;
        ContentCountReplacedUsingStringNWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\n", "").Length;
        ContentCountReplacedUsingStringRN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", " ").Length;
        ContentCountReplacedUsingStringRNWithEmpty = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", "").Length;
        ContentCountReplacedUsingStringRNWithN = data.TutoringLogistics == null ? -1 : data.TutoringLogistics.Replace("\r\n", "\n").Length;

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