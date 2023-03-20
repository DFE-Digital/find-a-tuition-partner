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

        public static void AddLadNameToAnalytics(this HttpContext httpContext, string? ladName)
        {
            if (!string.IsNullOrWhiteSpace(ladName))
            {
                httpContext.GetWebRequestEvent().AddData("LAD_Name", ladName);
            }
        }
    }
}

