using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Configuration;

public class ServiceUnavailableSettingsConfigure : IConfigureOptions<ServiceUnavailableSettings>
{
    private readonly IConfiguration _configuration;

    public ServiceUnavailableSettingsConfigure(
        IConfiguration configuration)
    {
        _configuration = configuration.GetSection(ServiceUnavailableSettings.ServiceUnavailableSettingsConfigName);
    }

    public void Configure(ServiceUnavailableSettings serviceUnavailableSettings)
    {
        serviceUnavailableSettings.GetServiceUnavailableSettings(_configuration);
    }
}