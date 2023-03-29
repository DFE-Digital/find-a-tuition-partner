
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

        public static string GetReferer(this HttpRequest request)
        {
            var referrer = string.Empty;

            try
            {
                referrer = request.Headers["Referer"].ToString();
            }
            catch { } //Suppress exception

            return referrer;
        }

        public static string GetNtpReferer(this HttpRequest request)
        {
            var ntpReferer = string.Empty;
            var referrerHost = GetRefererHost(request);

            if (!string.IsNullOrEmpty(referrerHost))
            {
                try
                {
                    if (string.Equals(referrerHost, request.Host.Host.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        ntpReferer = GetReferer(request);
                    }
                }
                catch { } //Suppress exception
            }

            return ntpReferer;
        }

        public static string GetRefererHost(this HttpRequest request)
        {
            var referrerHost = string.Empty;
            var referrer = GetReferer(request);

            if (!string.IsNullOrEmpty(referrer))
            {
                try
                {
                    var referrerUri = new Uri(referrer);
                    referrerHost = referrerUri.Host;
                }
                catch
                {
                    //Suppress exception
                    referrerHost = "Invalid Uri";
                }
            }

            return referrerHost;
        }
    }
}

