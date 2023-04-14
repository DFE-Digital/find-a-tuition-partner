
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;

namespace UI.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseServiceUrl(this HttpRequest request)
        {
            if (!request.IsHttps)
            {
                request.Scheme = "https";
            }
            return $"{request.Scheme}://{request.Host}{request.PathBase}";
        }

        public static string GetNtpRefererUrl(this HttpRequest request)
        {
            return request.GetNtpUrl(GetReferer(request));
        }

        public static string GetRefererHost(this HttpRequest request)
        {
            return GetHost(GetReferer(request));
        }

        public static string GetNtpUrlFromQuery(this HttpRequest request, string queryStringKey)
        {
            var queryString = request.Query[queryStringKey].ToString();

            return request.GetNtpUrl(queryString);
        }

        public static string GetFromUrlForQueryString(this HttpRequest request)
        {
            var currentPathAndQuery = request.GetEncodedUrl().Replace("http://", "https://");
            return $"FromReturnUrl={HttpUtility.UrlEncode(currentPathAndQuery)}";
        }

        private static string GetReferer(this HttpRequest request)
        {
            var referrer = string.Empty;

            try
            {
                referrer = request.Headers["Referer"].ToString();
            }
            catch { } //Suppress exception

            return referrer;
        }

        private static string GetNtpUrl(this HttpRequest request, string urlString)
        {
            var ntpUrl = string.Empty;

            try
            {
                var uri = GetUri(urlString);

                if (uri != null)
                {
                    if (string.Equals(uri.Host, request.Host.Host.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        ntpUrl = $"https://{uri.Authority}{uri.PathAndQuery}";
                    }
                }
            }
            catch { } //Suppress exception

            return ntpUrl;
        }

        private static string GetHost(string urlString)
        {
            var host = string.Empty;

            if (!string.IsNullOrEmpty(urlString))
            {
                try
                {
                    var uri = GetUri(urlString);

                    if (uri != null)
                        host = uri.Host;
                }
                catch
                {
                    //Suppress exception
                    host = "Invalid Uri";
                }
            }

            return host;
        }

        private static Uri? GetUri(string urlString)
        {
            Uri? uri = null;

            if (!string.IsNullOrEmpty(urlString))
            {
                uri = new Uri(urlString);
            }

            return uri;
        }
    }
}

