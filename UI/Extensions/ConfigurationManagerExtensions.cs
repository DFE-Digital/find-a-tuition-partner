using Infrastructure.Configuration;
using Infrastructure.Extensions;

namespace UI.Extensions
{
    public static class ConfigurationManagerExtensions
    {
        public static bool IsServiceUnavailable(this ConfigurationManager configuration)
        {
            var serviceUnavailableSettings = new ServiceUnavailableSettings();
            var section = configuration.GetSection(ServiceUnavailableSettings.ServiceUnavailableSettingsConfigName);
            serviceUnavailableSettings.GetServiceUnavailableSettings(section);
            return serviceUnavailableSettings.IsServiceUnavailable();
        }
    }
}
