using Application.Extensions;
using Notify.Exceptions;

namespace Infrastructure.Extensions;

public static class ExceptionExtensions
{
    public static bool IsNonCriticalNotifyException(this Exception ex)
    {
        if (ex is NotifyClientException)
        {
            var statusCode = ex.Message.GetGovNotifyStatusCodeFromExceptionMessage();
            return statusCode.Is4xxError();
        }

        return false;
    }
}