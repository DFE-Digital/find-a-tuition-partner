using System.Diagnostics;
using System.Net;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace UI.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {

        private readonly FeatureFlags _featureFlagsConfig;

        public bool IncludeEnquiryBuilder { get; set; } = true;
        public string? RequestId { get; set; }

        [BindProperty(SupportsGet = true)]
        public HttpStatusCode Status { get; set; }

        public ErrorModel(IOptions<FeatureFlags> featureFlagsConfig)
        {
            _featureFlagsConfig = featureFlagsConfig.Value;
        }

        public void OnGet()
        {
            if (TempData["Status"] is HttpStatusCode status)
            {
                Status = status;
            }
            IncludeEnquiryBuilder = _featureFlagsConfig.EnquiryBuilder;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}