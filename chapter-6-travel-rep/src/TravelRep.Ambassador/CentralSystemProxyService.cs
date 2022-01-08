using Hangfire;

namespace TravelRep.Ambassador
{
    public class CentralSystemProxyService : ICentralSystemProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SystemConfiguration _systemConfiguration;
        private readonly ILogger<CentralSystemProxyService> _logger;

        public CentralSystemProxyService(
            IHttpClientFactory httpClientFactory, SystemConfiguration systemConfiguration,
            ILogger<CentralSystemProxyService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _systemConfiguration = systemConfiguration;
            _logger = logger;
        }

        // Returns true for a successful call, 
        // and false to indicate that it will continue to try
        public async Task<bool> CallCheckin(double longitude, double latitude)
        {
            Console.WriteLine($"CallCheckin({longitude}, {latitude})");
            var result = await CallCentralSystemCheckin(longitude, latitude);
            if (result) return true;

            BackgroundJob.Enqueue(() =>           
                CallCentralSystemCheckinFireAndForget(longitude, latitude));
            
            return false;
        }

        [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Fail, 
            DelaysInSeconds = new[] { 1, 3, 20, 60, 3600 })]
        public async Task CallCentralSystemCheckinFireAndForget(double longitude, double latitude)
        {
            Console.WriteLine($"CallCentralSystemCheckinFireAndForget({longitude}, {latitude})");
            var result = await CallCentralSystemCheckin(longitude, latitude);
            if (result) throw new Exception("Unable to contact central system for checkin");
        }

        public async Task<bool> CallCentralSystemCheckin(double longitude, double latitude)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent("");
                string query = $"?longitude={longitude}&latitude={latitude}";
                var result = await client.PostAsJsonAsync($"{_systemConfiguration.CentralSystem}/checkin{query}", content);
                if (result.IsSuccessStatusCode) return true;

                var results = await result.Content.ReadAsStringAsync();
                _logger.LogWarning("Call failed:");
                _logger.LogWarning(results);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);                
                return false;
            }
        }
    }

    public interface ICentralSystemProxyService
    {
        Task<bool> CallCheckin(double longitude, double latitude);
    }
}
