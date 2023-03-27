
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
    }
}

