using System.Text.RegularExpressions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class PollEmailProcessing
    {
        private readonly ILogger<PollEmailProcessing> _logger;
        private readonly HttpClient _httpClient;

        public PollEmailProcessing(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<PollEmailProcessing>();
            _httpClient = httpClientFactory.CreateClient();
        }

        [Function("PollEmailProcessing")]
        public async Task RunAysnc([TimerTrigger("* * * * *")] MyInfo myTimer)
        {
            var url = Environment.GetEnvironmentVariable("PollEmailProcessingUrl", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("PollEmailProcessingUrl");

            _logger.LogInformation($"Find a Tuition Partner PollEmailProcessing function executed at: {DateTime.UtcNow}.  Polling {url}");

            var httpResponseMessage = await _httpClient.GetAsync(url);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var regex = new Regex(@"(?s)process-emails-outcome"">(.*?)</p>", RegexOptions.Multiline);

            var match = regex.Match(content);

            if (match.Success)
            {
                _logger.LogInformation(match.Groups[1].Value);
            }
            else
            {
                _logger.LogInformation($"Output Id not found, full content... {content}");
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
