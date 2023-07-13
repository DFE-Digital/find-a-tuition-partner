using System.Globalization;
using Application.Extensions;
using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Extensions
{
    public static class ServiceUnavailableSettingsExtensions
    {
        public static bool IsServiceUnavailable(this ServiceUnavailableSettings serviceUnavailableSettings)
        {
            return (serviceUnavailableSettings.StartDateTime == null ||
                serviceUnavailableSettings.StartDateTime < DateTime.UtcNow.ToLocalDateTime()) &&
                serviceUnavailableSettings.EndDateTime != null &&
                serviceUnavailableSettings.EndDateTime > DateTime.UtcNow.ToLocalDateTime();
        }

        public static void GetServiceUnavailableSettings(this ServiceUnavailableSettings serviceUnavailableSettings, IConfiguration configuration)
        {
            serviceUnavailableSettings.Message = configuration.GetValue<string>("Message");

            var startDateTime = configuration.GetValue<string>("StartDateTime");
            var endDateTime = configuration.GetValue<string>("EndDateTime");

            if (DateTime.TryParseExact(startDateTime, "dd/MM/yyyy HH:mm:ss", null,
                                                  DateTimeStyles.AssumeLocal, out DateTime start))
            {
                serviceUnavailableSettings.StartDateTime = start;
            }

            if (DateTime.TryParseExact(endDateTime, "dd/MM/yyyy HH:mm:ss", null,
                                                  DateTimeStyles.AssumeLocal, out DateTime end))
            {
                serviceUnavailableSettings.EndDateTime = end;
            }
        }
    }
}
