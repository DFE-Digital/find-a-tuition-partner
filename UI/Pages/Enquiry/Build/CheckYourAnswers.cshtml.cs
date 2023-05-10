using Application.Commands.Enquiry.Build;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Enquiry.Build;
using Domain.Exceptions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace UI.Pages.Enquiry.Build;

public class CheckYourAnswers : PageModel
{
    private const string EnquirySubmissionConfirmationModelKey = "EnquirySubmissionConfirmationModel";

    private readonly IMediator _mediator;
    private readonly ISessionService _sessionService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly FeatureFlags _featureFlagsConfig;
    private readonly ILogger<CheckYourAnswers> _logger;

    public CheckYourAnswers(IMediator mediator, ISessionService sessionService, IHostEnvironment hostEnvironment,
        IOptions<FeatureFlags> featureFlagsConfig, ILogger<CheckYourAnswers> logger)
    {
        _mediator = mediator;
        _sessionService = sessionService;
        _hostEnvironment = hostEnvironment;
        _featureFlagsConfig = featureFlagsConfig.Value;
        _logger = logger;
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

        Data.KeyStageSubjects = GetKeyStageSubject(Data.Subjects);
        Data.HasKeyStageSubjects = Data.KeyStageSubjects.Any();

        if (!string.IsNullOrWhiteSpace(Data.Postcode))
        {
            var locationResult = await _mediator.Send(new GetSearchLocationQuery(Data.Postcode));
            Data.LocalAuthorityDistrictName = locationResult == null ? string.Empty : locationResult.LocalAuthorityDistrict;
        }

        if (string.IsNullOrWhiteSpace(Data.LocalAuthorityDistrictName))
        {
            _logger.LogInformation("No LAD found on the CYA page for postcode: {postcode}", Data.Postcode);
            return NotFound();
        }

        HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrictName);

        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!_featureFlagsConfig.EnquiryBuilder)
            throw new InvalidOperationException("User is trying to submit an enquiry when the feature is disabled");

        var isDuplicateFormPost = !await _sessionService.IsDuplicateFormPostAsync();

        if (!await _sessionService.SessionDataExistsAsync() && !isDuplicateFormPost)
            return RedirectToPage("/Session/Timeout");

        Data.KeyStageSubjects = GetKeyStageSubject(Data.Subjects);

        if (!ModelState.IsValid) return Page();

        var searchResultsData = new GetSearchResultsQuery(Data);
        var searchResults = await _mediator.Send(searchResultsData);
        Data = Data with { TuitionPartnersForEnquiry = searchResults.Results };

        Data.BaseServiceUrl = Request.GetBaseServiceUrl();

        var submittedConfirmationModel = new SubmittedConfirmationModel();

        if (!isDuplicateFormPost)
        {
            var command = new AddEnquiryCommand()
            {
                Data = Data
            };

            await _sessionService.StartFormPostProcessing();

            try
            {
                submittedConfirmationModel = await _mediator.Send(command);
            }
            catch (EmailSendException)
            {
                submittedConfirmationModel.HadEmailSendException = true;
            }
            await _sessionService.SetFormPostResponse(submittedConfirmationModel, EnquirySubmissionConfirmationModelKey);
        }
        else
        {
            submittedConfirmationModel = await _sessionService.GetPreviousFormPostResponse<SubmittedConfirmationModel>(EnquirySubmissionConfirmationModelKey);
        }

        if (submittedConfirmationModel.HadEmailSendException)
        {
            Data.From = ReferrerList.CheckYourAnswers;

            var errorMessage =
                $"There was a problem sending the email and you should check the email address and try again";

            await _sessionService.AddOrUpdateDataAsync(SessionKeyConstants.EnquirerEmailErrorMessage, errorMessage);

            return RedirectToPage(nameof(EnquirerEmail), new SearchModel(Data));
        }

        await _sessionService.DeleteDataAsync();

        var submittedConfirmationModelRouteData = new SubmittedConfirmationModel(Data)
        {
            SupportReferenceNumber = submittedConfirmationModel.SupportReferenceNumber,
            LocalAuthorityDistrictName = Data.LocalAuthorityDistrictName
        };

        if (!_hostEnvironment.IsProduction())
        {
            submittedConfirmationModelRouteData.EnquirerMagicLink = submittedConfirmationModel.EnquirerMagicLink;

            submittedConfirmationModelRouteData.TuitionPartnerMagicLinksCount = submittedConfirmationModel.TuitionPartnerMagicLinksCount;

            var tuitionPartnerMagicLinksToDisplayForTesting = submittedConfirmationModel.TuitionPartnerMagicLinks.Take(10)
                .OrderBy(x => x.Email).ToList();

            submittedConfirmationModelRouteData.TuitionPartnerMagicLinks = tuitionPartnerMagicLinksToDisplayForTesting;
        }

        HttpContext.AddHasSENDQuestionToAnalytics<CheckYourAnswers>((!string.IsNullOrWhiteSpace(Data.SENDRequirements)).ToString());
        HttpContext.AddHasAdditionalInformationQuestionToAnalytics<CheckYourAnswers>((!string.IsNullOrWhiteSpace(Data.AdditionalInformation)).ToString());
        HttpContext.AddTuitionPartnerNameCsvAnalytics<CheckYourAnswers>(string.Join(",", Data.TuitionPartnersForEnquiry!.Results.Select(x => x.Name)));
        HttpContext.AddLadNameToAnalytics<CheckYourAnswers>(Data.LocalAuthorityDistrictName);
        HttpContext.AddEnquirySupportReferenceNumberToAnalytics<CheckYourAnswers>(submittedConfirmationModel.SupportReferenceNumber);

        return RedirectToPage(nameof(SubmittedConfirmation), submittedConfirmationModelRouteData);
    }

    private void ParseSessionValue(string key, string value)
    {
        switch (key)
        {
            case var k when k.Equals(SessionKeyConstants.EnquirerEmail, StringComparison.OrdinalIgnoreCase):
                Data.Email = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryTutoringLogistics, StringComparison.OrdinalIgnoreCase):
                Data.TutoringLogistics = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquirySENDRequirements, StringComparison.OrdinalIgnoreCase):
                Data.SENDRequirements = value;
                break;

            case var k when k.Equals(SessionKeyConstants.EnquiryAdditionalInformation, StringComparison.OrdinalIgnoreCase):
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