using Dfe.Analytics.AspNetCore;
using Domain.Search;

namespace UI.Extensions
{
    public static class HttpContextExtensions
    {
        public static void AddLadNameToAnalytics(this HttpContext httpContext, TuitionPartnersResult? result)
        {
            if (result != null)
            {
                httpContext.AddLadNameToAnalytics(result.LocalAuthorityDistrictName);
            }
        }

        public static void AddLadNameToAnalytics(this HttpContext httpContext, string? localAuthorityDistrictName)
        {
            if (httpContext != null && !string.IsNullOrWhiteSpace(localAuthorityDistrictName))
            {
                httpContext.GetWebRequestEvent().AddData("LocalAuthorityDistrictName", localAuthorityDistrictName);
            }
        }
    }
}

