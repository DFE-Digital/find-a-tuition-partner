using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class TutoringLogistics : PageModel
{
    private readonly ISessionService _sessionService;

    public TutoringLogistics(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [BindProperty] public TutoringLogisticsModel Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(TutoringLogisticsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        Data.TutoringLogisticsDetailsModel.NumberOfPupils = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryNumberOfPupils);
        Data.TutoringLogisticsDetailsModel.StartDate = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryStartDate);
        Data.TutoringLogisticsDetailsModel.TuitionDuration = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryTuitionDuration);
        Data.TutoringLogisticsDetailsModel.TimeOfDay = await _sessionService.RetrieveDataByKeyAsync(SessionKeyConstants.EnquiryTimeOfDay);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(TutoringLogisticsModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;
        if (ModelState.IsValid)
        {
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryNumberOfPupils, data.TutoringLogisticsDetailsModel.NumberOfPupils!);
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryStartDate, data.TutoringLogisticsDetailsModel.StartDate!);
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryTuitionDuration, data.TutoringLogisticsDetailsModel.TuitionDuration!);
            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquiryTimeOfDay, data.TutoringLogisticsDetailsModel.TimeOfDay!);

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(SENDRequirements), new SearchModel(data));
        }

        return Page();
    }
}