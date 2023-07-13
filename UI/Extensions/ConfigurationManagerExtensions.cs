using System.Globalization;
using Infrastructure.Configuration;
using Infrastructure.Extensions;

namespace UI.Extensions
{
    public static class ConfigurationManagerExtensions
    {
        public static bool IsServiceUnavailable(this ConfigurationManager configuration, string currentPath)
        {
            var startDateTime = configuration["ServiceUnavailableSettings:StartDateTime"];
            var endDateTime = configuration["ServiceUnavailableSettings:EndDateTime"];

            if (!string.IsNullOrEmpty(endDateTime))
            {
                if (currentPath != "/service-unavailable" &&
                    DateTime.TryParseExact(endDateTime, "MM/dd/yyyy HH:mm:ss", null,
                                              DateTimeStyles.AssumeLocal, out DateTime end))
                {
                    var hasStartDate = DateTime.TryParseExact(startDateTime, "MM/dd/yyyy HH:mm:ss", null,
                                              DateTimeStyles.AssumeLocal, out DateTime start);

                    var serviceUnavailableSettings = new ServiceUnavailableSettings()
                    {
                        StartDateTime = hasStartDate ? start : null,
                        EndDateTime = end
                    };

                    return serviceUnavailableSettings.IsServiceUnavailable();
                }
            }

            return false;
        }
    }
}
