using Dfe.Analytics.AspNetCore;
using Domain.Search;

namespace UI.Extensions
{
    public static class HttpContextExtensions
    {
        public static void AddLadNameToAnalytics<T>(this HttpContext httpContext, TuitionPartnersResult? result)
        {
            if (result != null)
            {
                httpContext.AddLadNameToAnalytics<T>(result.LocalAuthorityDistrictName);
            }
        }

        public static void AddLadNameToAnalytics<T>(this HttpContext httpContext, string? localAuthorityDistrictName)
        {
            httpContext.AddToAnalytics<T>("LocalAuthorityDistrictName", localAuthorityDistrictName);
        }

        public static void AddEnquirySupportReferenceNumberToAnalytics<T>(this HttpContext httpContext, string? enquirySupportReferenceNumber)
        {
            httpContext.AddToAnalytics<T>("EnquirySupportReferenceNumber", enquirySupportReferenceNumber);
        }

        public static void AddHasSENDQuestionToAnalytics<T>(this HttpContext httpContext, string? hasSENDQuestion)
        {
            httpContext.AddToAnalytics<T>("HasSENDQuestion", hasSENDQuestion);
        }

        public static void AddHasAdditionalInformationQuestionToAnalytics<T>(this HttpContext httpContext, string? hasAdditionalInformationQuestion)
        {
            httpContext.AddToAnalytics<T>("HasAdditionalInformationQuestion", hasAdditionalInformationQuestion);
        }

        public static void AddTuitionPartnerNameCsvAnalytics<T>(this HttpContext httpContext, string? tuitionPartnerNameCsv)
        {
            httpContext.AddToAnalytics<T>("TuitionPartnerNameCsv", tuitionPartnerNameCsv);
        }

        public static void AddSchoolUrnToAnalytics<T>(this HttpContext httpContext, int urn)
        {
            httpContext.AddToAnalytics<T>("SchoolUrn", urn.ToString());
        }

        private static void AddToAnalytics<T>(this HttpContext httpContext, string? key, string? value)
        {
            if (httpContext != null && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    httpContext.GetWebRequestEvent().AddData(key, value);
                }
                catch (Exception ex)
                {
                    var Logger = GetStaticLogger<T>();

                    Logger?.LogError("Error adding data to DfE Analytics. Error: {ex}", ex);
                }
            }
        }
    }
}

