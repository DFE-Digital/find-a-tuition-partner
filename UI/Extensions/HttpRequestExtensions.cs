
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace UI.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBaseServiceUrl(this HttpRequest request)
        {
            var scheme = request.IsHttps ? "https" : "http";
            var host = request.Headers.ContainsKey("X-Forwarded-Host") ? request.Headers["X-Forwarded-Host"].ToString() : request.Host.ToString();
            return $"{scheme}://{host}{request.PathBase}";
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
            var currentUrl = new Uri(request.GetEncodedUrl());
            var host = request.Headers.ContainsKey("X-Forwarded-Host") ? request.Headers["X-Forwarded-Host"].ToString() : currentUrl.Host;
            var builder = new UriBuilder(currentUrl)
            {
                Scheme = "https",
                Host = host
            };
            var currentPathAndQuery = builder.Uri.ToString();
            return $"FromReturnUrl={HttpUtility.UrlEncode(currentPathAndQuery)}";
        }


        private static string GetReferer(this HttpRequest request)
        {
            var referrer = string.Empty;

            try
            {
                referrer = request.Headers["Referer"].ToString();
            }
            catch
            {
                //Suppress exception
            }

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
                    var host = request.Headers.ContainsKey("X-Forwarded-Host") ? request.Headers["X-Forwarded-Host"].ToString() : request.Host.Host.ToString();
                    if (string.Equals(uri.Host, host, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ntpUrl = $"https://{uri.Authority}{uri.PathAndQuery}";
                    }
                }
            }
            catch
            {
                //Suppress exception
            }

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

