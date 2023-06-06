namespace Application.Extensions;
public static class IntExtensions
{
    public static bool Is4xxError(this int statusCode)
    {
        return statusCode >= 400 && statusCode < 500;
    }
}