using Application.Commands.Enquiry.Build;
using Application.Common.Models.Enquiry.Manage;
using Application.Queries.Enquiry;
using Application.Queries.Enquiry.Manage;
using UI.Extensions;

namespace UI.Pages.Enquiry.Manage
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class NotInterestedFeedback : PageModel
    {
        private readonly IMediator _mediator;
        [BindProperty] public NotInterestedFeedbackModel Data { get; set; } = new();
        [BindProperty] public EnquirerResponseResultsModel EnquirerResponseResultsModel { get; set; } = new();

        [FromRoute(Name = "support-reference-number")] public string SupportReferenceNumber { get; set; } = string.Empty;

        [FromRoute(Name = "tuition-partner-seo-url")] public string TuitionPartnerSeoUrl { get; set; } = string.Empty;

        public NotInterestedFeedback(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync(EnquirerResponseResultsModel enquirerResponseResultsModel)
        {
            var queryToken = Request.Query["Token"].ToString();

            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(queryToken, SupportReferenceNumber, TuitionPartnerSeoUrl)
                {
                    ValidateEnquiryResponseStatus = false
                });

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            var enquirerViewTuitionPartnerDetailsQuery = new GetEnquirerViewTuitionPartnerDetailsQuery(SupportReferenceNumber, TuitionPartnerSeoUrl);

            var data = await _mediator.Send(enquirerViewTuitionPartnerDetailsQuery);

            if (data == null)
            {
                return NotFound();
            }

            var enquirerNotInterestedReasons = await _mediator.Send(new GetEnquirerNotInterestedReasonsQuery());

            Data = new NotInterestedFeedbackModel()
            {
                SupportReferenceNumber = SupportReferenceNumber,
                TuitionPartnerSeoUrl = TuitionPartnerSeoUrl,
                Token = queryToken,
                TuitionPartnerName = data.TuitionPartnerName,
                EnquirerNotInterestedReasonModels = enquirerNotInterestedReasons
            };

            EnquirerResponseResultsModel = enquirerResponseResultsModel;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isValidMagicLink =
                await _mediator.Send(new IsValidMagicLinkTokenQuery(Data.Token, SupportReferenceNumber, TuitionPartnerSeoUrl)
                {
                    ValidateEnquiryResponseStatus = false
                });

            if (!isValidMagicLink)
            {
                return NotFound();
            }

            if (Data.EnquirerNotInterestedReasonId.HasValue)
            {
                Data.EnquirerNotInterestedReasonModels = await _mediator.Send(new GetEnquirerNotInterestedReasonsQuery());

                var selectedReason = Data.EnquirerNotInterestedReasonModels.Single(x => x.Id == Data.EnquirerNotInterestedReasonId!.Value);

                Data.MustCollectAdditionalInfo = selectedReason.CollectAdditionalInfoIfSelected;

                ModelState.ClearValidationState(nameof(Data));
                if (!TryValidateModel(Data, nameof(Data)))
                    return Page();

                await _mediator.Send(new UpdateNotInterestedReasonCommand(
                                    SupportReferenceNumber,
                                    TuitionPartnerSeoUrl,
                                    Data.EnquirerNotInterestedReasonId!.Value,
                                    selectedReason.Description,
                                    selectedReason.CollectAdditionalInfoIfSelected ? Data.EnquirerNotInterestedReasonAdditionalInfo : null));

                HttpContext.AddEnquirerNotInterestedReasonToAnalytics<NotInterestedFeedback>(selectedReason.Description);
            }

            var redirectPageUrl = $"/enquiry/{Data.SupportReferenceNumber}?Token={Data.Token}&{EnquirerResponseResultsModel.ToQueryString()}#all-responses-table";
            return Redirect(redirectPageUrl);
        }
    }
}