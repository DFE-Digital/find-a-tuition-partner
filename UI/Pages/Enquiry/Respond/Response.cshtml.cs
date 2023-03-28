using System.Net;
using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class Response : ResponsePageModel<Response>
    {
        public Response(IMediator mediator, ISessionService sessionService) : base(sessionService, mediator)
        {
        }

        [BindProperty] public ViewAndCaptureEnquiryResponseModel Data { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var queryToken = Request.Query["Token"].ToString();

            if (string.IsNullOrEmpty(SupportReferenceNumber) || string.IsNullOrEmpty(queryToken) || string.IsNullOrEmpty(TuitionPartnerSeoUrl))
            {
                TempData["Status"] = HttpStatusCode.NotFound;
                return RedirectToPage(nameof(ErrorModel));
            }

            Data.SupportReferenceNumber = SupportReferenceNumber;
            Data.TuitionPartnerSeoUrl = TuitionPartnerSeoUrl;
            Data.Token = queryToken;

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, true));

            if (!isValidMagicLink)
            {
                TempData["Status"] = HttpStatusCode.NotFound;
                return RedirectToPage(nameof(ErrorModel));
            }

            Data.BaseServiceUrl = Request.GetBaseServiceUrl();

            var enquiryData = await _mediator.Send(new
                GetEnquirerViewAllResponsesQuery(Data.BaseServiceUrl, SupportReferenceNumber));

            if (enquiryData == null)
            {
                TempData["Status"] = HttpStatusCode.NotFound;
                return RedirectToPage(nameof(ErrorModel));
            }

            Data.EnquiryResponseCloseDate = enquiryData.EnquiryCreatedDateTime.AddDays(IntegerConstants.EnquiryDaysToRespond);

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
                Data.EnquiryTuitionType = enquiryData.TuitionTypeName;
                Data.EnquiryTutoringLogistics = enquiryData.TutoringLogistics;
                Data.EnquirySENDRequirements = enquiryData.SENDRequirements;
                Data.EnquiryAdditionalInformation = enquiryData.AdditionalInformation;

                HttpContext.AddLadNameToAnalytics<Response>(Data.LocalAuthorityDistrict);
                HttpContext.AddEnquirySupportReferenceNumberToAnalytics<Response>(Data.SupportReferenceNumber);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var queryToken = Request.Query["Token"].ToString();

            try
            {
                var isValidMagicLink =
                    await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, true));

                if (!isValidMagicLink)
                {
                    TempData["Status"] = HttpStatusCode.NotFound;
                    return RedirectToPage(nameof(ErrorModel));
                }

                Data.BaseServiceUrl = Request.GetBaseServiceUrl();

                Data.Token = queryToken;

                await _sessionService.AddOrUpdateDataAsync(new Dictionary<string, string>()
                {
                    { SessionKeyConstants.LocalAuthorityDistrict, Data.LocalAuthorityDistrict! },
                    { SessionKeyConstants.EnquiryResponseTutoringLogistics, Data.TutoringLogisticsText! },
                    { SessionKeyConstants.EnquiryResponseKeyStageAndSubjectsText, Data.KeyStageAndSubjectsText! },
                    { SessionKeyConstants.EnquiryResponseTuitionTypeText, Data.TuitionTypeText! },
                    { SessionKeyConstants.EnquiryResponseSENDRequirements, Data.SENDRequirementsText ?? string.Empty },
                    {
                        SessionKeyConstants.EnquiryResponseAdditionalInformation,
                        Data.AdditionalInformationText ?? string.Empty
                    },
                    { SessionKeyConstants.EnquiryResponseToken, Data.Token! },
                    { SessionKeyConstants.EnquiryKeyStageSubjects, string.Join(Environment.NewLine, Data.EnquiryKeyStageSubjects!) },
                    { SessionKeyConstants.EnquiryTuitionType, Data.EnquiryTuitionType! },
                    { SessionKeyConstants.EnquiryTutoringLogistics, Data.EnquiryTutoringLogistics! },
                    { SessionKeyConstants.EnquirySENDRequirements, Data.EnquirySENDRequirements ?? string.Empty },
                    { SessionKeyConstants.EnquiryAdditionalInformation, Data.EnquiryAdditionalInformation ?? string.Empty }
                },
                GetSessionKey(Data.TuitionPartnerSeoUrl!, Data.SupportReferenceNumber)
                );

                return RedirectToPage(nameof(CheckYourAnswers), new CheckYourAnswersModel()
                {
                    SupportReferenceNumber = Data.SupportReferenceNumber,
                    TuitionPartnerSeoUrl = Data.TuitionPartnerSeoUrl
                });
            }
            catch
            {
                return Page();
            }
        }

    }
}