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
            httpContext.AddToAnalytics("LocalAuthorityDistrictName", localAuthorityDistrictName);
        }

        public static void AddTuitionPartnerNameToAnalytics(this HttpContext httpContext, string? tuitionPartnerName)
        {
            httpContext.AddToAnalytics("TuitionPartnerName", tuitionPartnerName);
        }

        public static void AddEnquirySupportReferenceNumberToAnalytics(this HttpContext httpContext, string? enquirySupportReferenceNumber)
        {
            httpContext.AddToAnalytics("EnquirySupportReferenceNumber", enquirySupportReferenceNumber);
        }


        public static void AddToAnalytics(this HttpContext httpContext, string? key, string? value)
        {
            if (httpContext != null && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                httpContext.GetWebRequestEvent().AddData(key, value);
            }
        }
    }
}

