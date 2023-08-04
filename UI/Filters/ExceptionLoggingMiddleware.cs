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
                var scheme = context.Request.Scheme;
                var host = context.Request.Headers.ContainsKey("X-Forwarded-Host") ? context.Request.Headers["X-Forwarded-Host"].ToString() : context.Request.Host.ToString();
                var path = context.Request.Path;
                var queryString = context.Request.QueryString;

                var url = $"{scheme}://{host}{path}{queryString}";

                var additionalComment = string.Empty;
                var ntpReferer = context.Request.GetNtpRefererUrl();
                if (!string.IsNullOrWhiteSpace(ntpReferer))
                {
                    //Internal URL then log it
                    additionalComment = $"  The referer is {ntpReferer}";
                }
                else
                {
                    //Else log the external host if there is one
                    var refererHost = context.Request.GetRefererHost();
                    if (!string.IsNullOrWhiteSpace(refererHost))
                    {
                        additionalComment = $"  The referer host is {refererHost}";
                    }
                }
                //Log 404 errors so we can review these, if needed
                _logger.LogInformation("404 Response for {url}.{AdditionalComment}", url, additionalComment);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception in HTTP pipeline");
            throw;
        }
    }
}