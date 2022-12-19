namespace UI.Filters;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        => (_next, _logger) = (next, logger);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == 404)
            {
                var url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
                var referrer = context.Request.Headers["Referer"].ToString();
                var referrerComment = string.Empty;
                if (!string.IsNullOrWhiteSpace(referrer))
                {
                    var referrerUri = new Uri(referrer);
                    referrerComment = $"  The referrer host is {referrerUri.Host}";
                }
                //Log 404 errors so we can review these, if needed
                _logger.LogInformation("404 Response for {url}.{RerererComment}", url, referrerComment);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception in HTTP pipeline");
            throw;
        }
    }
}