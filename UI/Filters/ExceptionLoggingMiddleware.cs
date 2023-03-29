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

                var additionalComment = string.Empty;
                var ntpReferer = context.Request.GetNtpReferer();
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