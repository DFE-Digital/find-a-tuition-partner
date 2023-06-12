using Application.Common.Models;

namespace UI.Pages.Enquiry.Build
{
    public class Guidance : PageModel
    {
        private readonly IMediator _mediator;

        public Guidance(IMediator mediator)
        {
            _mediator = mediator;
        }

        public SearchModel Data { get; set; } = new();
        public bool HasValidSchoolPostcode { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(SearchModel data)
        {
            Data = data;

            if (!string.IsNullOrWhiteSpace(Data.Postcode))
            {
                var locationResult = await _mediator.Send(new GetSearchLocationQuery(Data.Postcode));
                HasValidSchoolPostcode = locationResult.TryValidate(true).IsSuccess;
            }

            return Page();
        }
    }
}
