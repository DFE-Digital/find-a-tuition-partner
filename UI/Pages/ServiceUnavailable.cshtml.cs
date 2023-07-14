using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace UI.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ServiceUnavailable : PageModel
    {
        private readonly ILogger<ServiceUnavailable> _logger;
        private readonly ServiceUnavailableSettings _serviceUnavailableSettingsConfig;

        public string? Message { get; set; }

        public ServiceUnavailable(IOptions<ServiceUnavailableSettings> serviceUnavailableSettingsConfig, ILogger<ServiceUnavailable> logger)
        {
            _serviceUnavailableSettingsConfig = serviceUnavailableSettingsConfig.Value;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!_serviceUnavailableSettingsConfig.IsServiceUnavailable())
            {
                return Redirect(nameof(Index));
            }

            _logger.LogWarning("The service is currently unavailable until {EndDateTime}", _serviceUnavailableSettingsConfig.EndDateTime!.Value.ToString(StringConstants.DateTimeFormatGDS));

            Message = _serviceUnavailableSettingsConfig.Message;

            if (_serviceUnavailableSettingsConfig.EndDateTime.HasValue)
            {
                Message = Message
                    .Replace("{EndDateTime}", _serviceUnavailableSettingsConfig.EndDateTime!.Value.ToString(StringConstants.DateTimeFormatGDS));
            }

            if (_serviceUnavailableSettingsConfig.StartDateTime.HasValue)
            {
                Message = Message
                    .Replace("{StartDateTime}", _serviceUnavailableSettingsConfig.StartDateTime!.Value.ToString(StringConstants.DateTimeFormatGDS));
            }

            return Page();
        }
    }
}