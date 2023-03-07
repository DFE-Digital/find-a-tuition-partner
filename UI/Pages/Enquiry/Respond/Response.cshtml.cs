using Application.Common.Interfaces;
using Application.Common.Models.Enquiry.Respond;

namespace UI.Pages.Enquiry.Respond
{
    public class Response : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IEncrypt _aesEncrypt;

        private const string InvalidTokenErrorMessage = "Invalid token provided in the URl.";

        private const string InvalidUrlErrorMessage = "Invalid Url";

        public Response(IMediator mediator, IEncrypt aesEncrypt)
        {
            _mediator = mediator;
            _aesEncrypt = aesEncrypt;
        }

        [BindProperty] public EnquiryResponseModel Data { get; set; } = new();

        [ViewData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            /*
            TODO:
            Once confirmed update the error content and logic if:
                No token supplied; 
                Invalid URL/token supplied (possibly same message as previous error);
                Token has expired; - The expiry date is stored in db, but not inspected on this page at the moment, so is not being used.
                The enquiry has already been answered (I assume error message is shown for MVP rather than a read only version of the answers supplied?) - how to deal with this?  Possibly after validated the magic link is OK ensure that the response answers are empty?
            Ensure that the input fields are not shown on page if the above happens.
            Ensure that the above happens on all magic link pages - this authorisation kind of logic might apply to all these response pages?
            Update the magic links for TPs to be 7 days, confirm what to set for the enquiry as initial expiry date
            */

            var token = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(token)) return Page();

            try
            {
                GetTokenValues(token);

                var validMagicLinkToken = await IsValidMagicLinkToken(token);
                if (!validMagicLinkToken) return Page();
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var token = Request.Query["token"].ToString();

            if (AddInValidUrlErrorMessage(token)) return Page();

            try
            {
                GetTokenValues(token);

                var validMagicLinkToken = await IsValidMagicLinkToken(token);
                if (!validMagicLinkToken) return Page();

                Data.BaseServiceUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                var command = new AddEnquiryResponseCommand()
                {
                    Data = Data
                };

                var hasDataSaved = await _mediator.Send(command);

                if (hasDataSaved)
                {
                    return RedirectToPage(nameof(ResponseConfirmation));
                }
            }
            catch
            {
                AddErrorMessage(InvalidTokenErrorMessage);
                return Page();
            }

            return Page();
        }

        private void GetTokenValues(string token)
        {
            var tokenValue = _aesEncrypt.Decrypt(token);

            var splitTokenValue = tokenValue.Split('&', StringSplitOptions.RemoveEmptyEntries);

            var splitEnquiryPart = splitTokenValue[0].Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(splitEnquiryPart[1], out var enquiryId))
            {
                Data.EnquiryId = enquiryId;
            }

            var splitTuitionPartnerPart = splitTokenValue[1].Split('=', StringSplitOptions.RemoveEmptyEntries);

            if (int.TryParse(splitTuitionPartnerPart[1], out var tuitionPartnerId))
            {
                Data.TuitionPartnerId = tuitionPartnerId;
            }

            Data.Token = token;
        }

        private void AddErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;

            ModelState.AddModelError("Data.ErrorMessage", ErrorMessage);
        }

        private bool AddInValidUrlErrorMessage(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                AddErrorMessage(InvalidUrlErrorMessage);
                return true;
            }

            return false;
        }

        private async Task<bool> IsValidMagicLinkToken(string token)
        {
            var isValidMagicLinkToken = await _mediator.Send(new IsValidMagicLinkTokenQuery(token, nameof(MagicLinkType.EnquiryRequest)));

            if (!isValidMagicLinkToken)
            {
                AddErrorMessage(InvalidTokenErrorMessage);

                return false;
            }

            return true;
        }
    }
}