using Hangfire;

namespace TravelRep.Ambassador
{
    public class CentralSystemProxyService : ICentralSystemProxyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SystemConfiguration _systemConfiguration;

        public CentralSystemProxyService(
            IHttpClientFactory httpClientFactory, SystemConfiguration systemConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _systemConfiguration = systemConfiguration;
        }

        // Returns true for a successful call, 
        // and false to indicate that it will continue to try
        public async Task<bool> CallCheckin(double longitude, double latitude)
        {
            var result = await CallCentralSystemCheckin(longitude, latitude);
            if (result.IsSuccessStatusCode) return true;

            BackgroundJob.Enqueue(() =>           
                CallCentralSystemCheckin(longitude, latitude));
            return false;
        }

        public async Task<HttpResponseMessage> CallCentralSystemCheckin(double longitude, double latitude)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent("");
            string query = $"?longitude={longitude}&latitude={latitude}";
            var result = await client.PostAsJsonAsync($"{_systemConfiguration.CentralSystem}/checkin{query}", content);
            return result;
        }
    }

    public interface ICentralSystemProxyService
    {
        Task<bool> CallCheckin(double longitude, double latitude);
    }
}
