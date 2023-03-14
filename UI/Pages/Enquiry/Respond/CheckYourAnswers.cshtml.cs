using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond;

public class CheckYourAnswers : PageModel
{
    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;

    public CheckYourAnswers(ISessionService sessionService, IMediator mediator)
    {
        _sessionService = sessionService;
        _mediator = mediator;
    }

    public async Task<IActionResult> OnGetAsync(CheckYourAnswersModel data)
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data = data;

        var sessionValues = await _sessionService.RetrieveDataAsync();

        if (sessionValues != null)
        {
            foreach (var sessionValue in sessionValues)
            {
                ParseSessionValue(sessionValue.Key, sessionValue.Value);
            }
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        if (!ModelState.IsValid) return Page();

        var command = new AddEnquiryResponseCommand()
        {
            Data = Data
        };

        var supportReferenceNumber = await _mediator.Send(command);

        if (!string.IsNullOrEmpty(supportReferenceNumber))
        {
            Data.SupportReferenceNumber = supportReferenceNumber;

            await _sessionService.DeleteDataAsync();

            return RedirectToPage(nameof(ResponseConfirmation), new SearchModel(Data));
        }

        return Page();
    }

    private void ParseSessionValue(string key, string value)
    {
        switch (key)
        {
            case var k when k.Contains(StringConstants.LocalAuthorityDistrict):
                Data.LocalAuthorityDistrict = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryTutoringLogistics):
                Data.EnquiryTutoringLogistics = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseTutoringLogistics):
                Data.TutoringLogisticsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryKeyStageSubjects):
                Data.EnquiryKeyStageSubjects = value.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseKeyStageAndSubjectsText):
                Data.KeyStageAndSubjectsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryTuitionType):
                Data.EnquiryTuitionType = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseTuitionTypeText):
                Data.TuitionTypeText = value;
                break;

            case var k when k.Contains(StringConstants.EnquirySENDRequirements):
                Data.EnquirySENDRequirements = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseSENDRequirements):
                Data.SENDRequirementsText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryAdditionalInformation):
                Data.EnquiryAdditionalInformation = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseAdditionalInformation):
                Data.AdditionalInformationText = value;
                break;

            case var k when k.Contains(StringConstants.EnquiryResponseToken):
                Data.Token = value;
                break;
        }
    }
}