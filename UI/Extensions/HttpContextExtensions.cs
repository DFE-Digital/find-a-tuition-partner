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
            httpContext.AddToAnalytics("LocalAuthorityDistrictName", localAuthorityDistrictName, null);
        }

        public static void AddLadNameToAnalytics(this HttpContext httpContext, string? localAuthorityDistrictName, ILogger? logger)
        {
            httpContext.AddToAnalytics("LocalAuthorityDistrictName", localAuthorityDistrictName, logger);
        }

        public static void AddTuitionPartnerNameToAnalytics(this HttpContext httpContext, string? tuitionPartnerName)
        {
            httpContext.AddToAnalytics("TuitionPartnerName", tuitionPartnerName, null);
        }

        public static void AddEnquirySupportReferenceNumberToAnalytics(this HttpContext httpContext, string? enquirySupportReferenceNumber)
        {
            httpContext.AddToAnalytics("EnquirySupportReferenceNumber", enquirySupportReferenceNumber, null);
        }

        public static void AddHasSENDQuestionToAnalytics(this HttpContext httpContext, string? hasSENDQuestion)
        {
            httpContext.AddToAnalytics("HasSENDQuestion", hasSENDQuestion, null);
        }

        public static void AddHasAdditionalInformationQuestionToAnalytics(this HttpContext httpContext, string? hasAdditionalInformationQuestion)
        {
            httpContext.AddToAnalytics("HasAdditionalInformationQuestion", hasAdditionalInformationQuestion, null);
        }

        public static void AddTuitionPartnerNameCsvAnalytics(this HttpContext httpContext, string? tuitionPartnerNameCsv)
        {
            httpContext.AddToAnalytics("TuitionPartnerNameCsv", tuitionPartnerNameCsv, null);
        }

        private static void AddToAnalytics(this HttpContext httpContext, string? key, string? value, ILogger? logger)
        {
            if (httpContext != null && !string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    httpContext.GetWebRequestEvent().AddData(key, value);
                }
                catch (Exception ex)
                {
                    logger?.LogError("Error adding data to DfE Analytics", ex);
                }
            }
        }
    }
}

