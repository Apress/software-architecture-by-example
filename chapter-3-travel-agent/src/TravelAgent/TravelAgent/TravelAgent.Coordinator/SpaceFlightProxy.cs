using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Coordinator
{
    public class SpaceFlightProxy
    {
        private IHttpClientFactory _httpClientFactory;

        public SpaceFlightProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CallSpaceFlightApi(DateTime date) =>
            await BookSpaceFlight(date);            

        private async Task<bool> BookSpaceFlight(DateTime date)
        {
            var client = _httpClientFactory.CreateClient();

            string requestString = "{ \"date\": \"" + date.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\", \"roomType\": 1 }";

            var requestContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json");
            var result = await client.PostAsync("https://localhost:5101/book", requestContent);
            return result.IsSuccessStatusCode;
        }

    }
}
