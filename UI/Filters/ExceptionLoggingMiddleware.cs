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
                using var _ = _logger.BeginScope("{@Context}", context);
                var url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
                var referer = context.Request.Headers["Referer"].ToString();
                var rerererComment = string.IsNullOrWhiteSpace(referer) ? string.Empty : $"  The referer was {referer}";
                //Log 404 errors so we can review these, if needed
                _logger.LogInformation("404 Response for {url}.{RerererComment}", url, rerererComment);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception in HTTP pipeline");
            throw;
        }
    }
}