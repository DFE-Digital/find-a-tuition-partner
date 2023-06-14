using Application.Common.Models.Enquiry.Respond;
using Application.Queries.Enquiry;

namespace UI.Pages.Enquiry.Respond
{
    public class ViewResponse : PageModel
    {
        protected readonly IMediator _mediator;

        public ViewResponse(IMediator mediator)
        {
            _mediator = mediator;
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

            Data.LocalAuthorityDistrict = enquiryData.LocalAuthorityDistrict!;
            Data.EnquiryKeyStageSubjects = enquiryData.KeyStageSubjects;
            Data.EnquiryTuitionSetting = enquiryData.TuitionSettingName;
            Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogistics = enquiryData.TutoringLogisticsDisplayModel.TutoringLogistics;
            Data.EnquiryTutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel = enquiryData.TutoringLogisticsDisplayModel.TutoringLogisticsDetailsModel;
            Data.EnquirySENDRequirements = enquiryData.SENDRequirements;
            Data.EnquiryAdditionalInformation = enquiryData.AdditionalInformation;

            Data.NumberOfTpEnquiryWasSent = enquiryData.NumberOfTpEnquiryWasSent;

            HttpContext.AddLadNameToAnalytics<ViewResponse>(Data.LocalAuthorityDistrict);

            return Page();
        }
    }
}