using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class ConfirmSchool : PageModel
{
    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;

    public ConfirmSchool(ISessionService sessionService, IMediator mediator)
    {
        _sessionService = sessionService;
        _mediator = mediator;
    }
    [BindProperty] public ConfirmSchoolModel Data { get; set; } = new();

    public bool FromSchoolPostcode { get; set; } = false;

    public async Task<IActionResult> OnGetAsync(ConfirmSchoolModel data)
    {
        Data = data;

        var isValid = await ValidateAndSetData(data);

        if (!isValid)
            return RedirectToPage(nameof(SchoolPostcode), data);

        var schoolId = await _sessionService.GetAsync<int?>(SessionKeyConstants.EnquirySchoolId);

        if (schoolId.HasValue &&
            !data.Schools.Any(x => x.Id == schoolId.Value))
            schoolId = null;

        Data.SchoolId = schoolId;

        ModelState.Clear();

        return Page();
    }
    public async Task<IActionResult> OnPostAsync(ConfirmSchoolModel data)
    {
        Data = data;

        await ValidateAndSetData(data);

        if (ModelState.IsValid)
        {
            if (data.HasSingleSchool)
            {
                if (data.ConfirmedIsSchool!.Value)
                {
                    await _sessionService.SetAsync(SessionKeyConstants.EnquirySchoolId, data.Schools.First().Id);
                }
                else
                {
                    return RedirectToPage(nameof(SchoolPostcode), data);
                }
            }
            else
            {
                await _sessionService.SetAsync(SessionKeyConstants.EnquirySchoolId, data.SchoolId!.Value);
            }

            if (!string.IsNullOrEmpty(data.SchoolPostcode))
            {
                data.Postcode = data.SchoolPostcode;
            }

            if (data.From == ReferrerList.CheckYourAnswers)
            {
                return RedirectToPage(nameof(CheckYourAnswers), new SearchModel(data));
            }

            return RedirectToPage(nameof(EnquirerEmail), new SearchModel(data));
        }

        return Page();
    }

    private async Task<bool> ValidateAndSetData(ConfirmSchoolModel data)
    {
        var postcode = data.Postcode;

        if (!string.IsNullOrEmpty(data.SchoolPostcode))
        {
            postcode = data.SchoolPostcode;
            FromSchoolPostcode = true;
        }

        if (!string.IsNullOrWhiteSpace(postcode))
        {
            var locationResult = await _mediator.Send(new GetSearchLocationQuery(postcode));
            if (!locationResult.TryValidate(true).IsSuccess ||
                locationResult == null ||
                locationResult.Schools == null ||
                !locationResult.Schools.Any())
            {
                return false;
            }
            else
            {
                data.Schools = locationResult.Schools;
                data.HasSingleSchool = data.Schools.Count == 1;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}