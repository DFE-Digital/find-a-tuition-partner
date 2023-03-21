using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;

namespace UI.Pages.Enquiry.Build;

public class CheckYourAnswers : PageModel
{
    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly IHostEnvironment _hostEnvironment;

    public CheckYourAnswers(IMediator mediator, ISessionService sessionService, IHostEnvironment hostEnvironment)
    {
        _mediator = mediator;
        _sessionService = sessionService;
        _hostEnvironment = hostEnvironment;
    }

    [BindProperty] public CheckYourAnswersModel Data { get; set; } = new();

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

        //TODO - Test and handle errors:
        //  No postcode, subjects, TT, email, logistics etc
        //  Invalid data supplied - postcode in Wales, invalid email etc
        //  errors when calling _mediator

        Data.KeyStageSubjects = GetKeyStageSubject(Data.Subjects);
        Data.HasKeyStageSubjects = Data.KeyStageSubjects.Any();

        if (!string.IsNullOrWhiteSpace(Data.Postcode))
        {
            var locationResult = await _mediator.Send(new GetSearchLocationQuery(Data.Postcode));
            Data.LocalAuthorityDistrictName = locationResult == null ? string.Empty : locationResult.LocalAuthorityDistrict;
        }

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _sessionService.SessionDataExistsAsync())
            return RedirectToPage("/Session/Timeout");

        Data.KeyStageSubjects = GetKeyStageSubject(Data.Subjects);

        if (!ModelState.IsValid) return Page();

        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var command = new AddEnquiryCommand()
        {
            Data = Data
        };

        var submittedConfirmationModel = await _mediator.Send(command);

        var enquirerEmailSentStatus = submittedConfirmationModel.EnquirerEmailSentStatus;

        if (!string.IsNullOrEmpty(enquirerEmailSentStatus))
        {
            if (enquirerEmailSentStatus == StringConstants.EnquirerEmailSentStatus4xxErrorValue)
            {
                Data.From = ReferrerList.CheckYourAnswers;

                var errorMessage =
                    $"There was a problem sending the email and you should check the email address and try again";

                await _sessionService.AddOrUpdateDataAsync(StringConstants.EnquirerEmailErrorMessage, errorMessage);

                return RedirectToPage(nameof(EnquirerEmail), new SearchModel(Data));
            }

            if (enquirerEmailSentStatus == StringConstants.EnquirerEmailSentStatus5xxErrorValue)
            {
                return RedirectToPage(nameof(ErrorModel));
            }
        }

        if (!string.IsNullOrEmpty(submittedConfirmationModel.SupportReferenceNumber))
        {
            await _sessionService.DeleteDataAsync();

            var submittedConfirmationModelRouteData = new SubmittedConfirmationModel(Data)
            {
                SupportReferenceNumber = submittedConfirmationModel.SupportReferenceNumber
            };

            if (!_hostEnvironment.IsProduction())
            {
                submittedConfirmationModelRouteData.EnquirerMagicLink = submittedConfirmationModel.EnquirerMagicLink;
                submittedConfirmationModelRouteData.TuitionPartnerMagicLinks = submittedConfirmationModel.TuitionPartnerMagicLinks.OrderBy(x => x.Key).Take(10).ToDictionary(pair => pair.Key, pair => pair.Value);
                submittedConfirmationModelRouteData.TuitionPartnerMagicLinksCount = submittedConfirmationModel.TuitionPartnerMagicLinks.Count;
            }

            return RedirectToPage(nameof(SubmittedConfirmation), submittedConfirmationModelRouteData);
        }

        return Page();
    }

    private void ParseSessionValue(string key, string value)
    {
        switch (key)
        {
            case var k when k.Equals(StringConstants.EnquirerEmail, StringComparison.OrdinalIgnoreCase):
                Data.Email = value;
                break;

            case var k when k.Equals(StringConstants.EnquiryTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                Data.TutoringLogistics = value;
                break;

            case var k when k.Equals(StringConstants.EnquirySENDRequirements, StringComparison.OrdinalIgnoreCase):
                Data.SENDRequirements = value;
                break;

            case var k when k.Equals(StringConstants.EnquiryAdditionalInformation, StringComparison.OrdinalIgnoreCase):
                Data.AdditionalInformation = value;
                break;
        }
    }
    private static Dictionary<KeyStage, List<Subject>> GetKeyStageSubject(string[]? subjects)
    {
        if (subjects == null || subjects.Length == 0)
            return new Dictionary<KeyStage, List<Subject>>();

        var keyStageSubjects = subjects.ParseKeyStageSubjects() ?? Array.Empty<KeyStageSubject>();

        var allSubjects = Enum.GetValues(typeof(Subject)).Cast<Subject>();

        return keyStageSubjects
            .GroupBy(x => x.KeyStage)
            .OrderBy(x => x.Key.DisplayName())
            .Select(x => new
            {
                x.Key,
                Values = x
                .Select(y => allSubjects
                .FirstOrDefault(allsub => allsub.ToString().ToSeoUrl() == y.Subject.ToSeoUrl()))
                .Where(x => x != Subject.Unspecified)
            })
            .Where(x => x.Values.Any())
            .ToDictionary(x => x.Key, x => x.Values.ToList());
    }
}