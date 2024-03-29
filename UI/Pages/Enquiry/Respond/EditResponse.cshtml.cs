using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;
using Application.Queries.Enquiry;
using UI.Models;

namespace UI.Pages.Enquiry.Respond
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class EditResponse : ResponsePageModel<EditResponse>
    {
        public EditResponse(IMediator mediator, ISessionService sessionService) : base(sessionService, mediator)
        {
        }

        [BindProperty] public ViewAndCaptureEnquiryResponseModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl, true));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            Data.SupportReferenceNumber = SupportReferenceNumber;
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
            Data.Token = queryToken;

            var enquiryData = await _mediator.Send(new GetEnquirerViewAllResponsesQuery(SupportReferenceNumber));

            Data.EnquiryResponseCloseDateFormatted = enquiryData.EnquiryCreatedDateTime.AddDays(IntegerConstants.EnquiryDaysToRespond).ToString("h:mmtt 'on' dddd d MMMM yyyy");

            var sessionValues = await _sessionService.RetrieveDataAsync(GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber));

            if (sessionValues != null)
            {
                foreach (var sessionValue in sessionValues)
                {
                    Data.EnquiryResponseParseSessionValues(sessionValue.Key, sessionValue.Value);
                }
            }
            else
            {
                Data.LocalAuthorityDistrict = enquiryData.LocalAuthorityDistrict!;
                Data.EnquiryKeyStageSubjects = enquiryData.KeyStageSubjects;
                Data.EnquiryTuitionSetting = enquiryData.TuitionSettingName;
                Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics = enquiryData.TutoringLogisticsDisplayModel.TutoringLogistics;
                Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel = enquiryData.TutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel;
                Data.EnquirySENDRequirements = enquiryData.SENDRequirements;
                Data.EnquiryAdditionalInformation = enquiryData.AdditionalInformation;
            }

            HttpContext.AddLadNameToAnalytics<EditResponse>(Data.LocalAuthorityDistrict);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, Data.SupportReferenceNumber, Data.TuitionPartnerSeoUrl, true));

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel = Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics.ToTutoringLogisticsDetailsModel();
                return Page();
            }

            await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
            {
                { SessionKeyConstants.LocalAuthorityDistrict, Data.LocalAuthorityDistrict! },
                { SessionKeyConstants.EnquiryResponseTutoringLogistics, Data.TutoringLogisticsText! },
                { SessionKeyConstants.EnquiryResponseKeyStageAndSubjectsText, Data.KeyStageAndSubjectsText! },
                { SessionKeyConstants.EnquiryResponseTuitionSettingText, Data.TuitionSettingText! },
                { SessionKeyConstants.EnquiryResponseSENDRequirements, Data.SENDRequirementsText ?? string.Empty },
                {
                    SessionKeyConstants.EnquiryResponseAdditionalInformation,
                    Data.AdditionalInformationText ?? string.Empty
                },
                { SessionKeyConstants.EnquiryKeyStageSubjects, string.Join(Environment.NewLine, Data.EnquiryKeyStageSubjects!) },
                { SessionKeyConstants.EnquiryTuitionSetting, Data.EnquiryTuitionSetting! },
                { SessionKeyConstants.EnquiryTutoringLogistics, Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics! },
                { SessionKeyConstants.EnquirySENDRequirements, Data.EnquirySENDRequirements ?? string.Empty },
                { SessionKeyConstants.EnquiryAdditionalInformation, Data.EnquiryAdditionalInformation ?? string.Empty }
            },
            GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber)
            );

            var redirectPageUrl = $"/enquiry-response/{Data.TuitionPartnerSeoUrl}/{Data.SupportReferenceNumber}/check-your-answers?Token={Data.Token}";
            return Redirect(redirectPageUrl);
        }
    }
}