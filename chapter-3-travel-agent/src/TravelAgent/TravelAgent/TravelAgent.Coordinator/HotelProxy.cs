using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Coordinator
{
    public class HotelProxy
    {
        private IHttpClientFactory _httpClientFactory;

        public HotelProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CallHotelApi(DateTime date)
        {
            bool isAvailable = await GetHotelAvailability(date);
            if (isAvailable)
            {
                return await BookHotelRoom(date);
            }

            return false;
        }

        private async Task<bool> GetHotelAvailability(DateTime dateTime)
        {
            var client = _httpClientFactory.CreateClient();            

            string dateParam = dateTime.ToString("yyyy-MM-dd");
            var result = await client.GetAsync($"https://localhost:5020/Availability?date={dateParam}");
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var jobject = JObject.Parse(content);
                var available = jobject.SelectToken("roomCount");
                bool isAvailable = available.Value<int>() >= 1;

                return isAvailable;
            }

            return false;
        }

        private async Task<bool> BookHotelRoom(DateTime date)
        {
            var client = _httpClientFactory.CreateClient();

            string requestString = "{ \"date\": \"" + date.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\", \"roomType\": 1 }";

            var requestContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json");
            var result = await client.PostAsync("https://localhost:5020/book", requestContent);
            return result.IsSuccessStatusCode;
        }

        internal async Task<bool> CancelHotelRoom(DateTime date)
        {
            var client = _httpClientFactory.CreateClient();

            string requestString = "{ \"date\": \"" + date.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\", \"roomType\": 1 }";

            var requestContent = new StringContent(
                requestString,
                Encoding.UTF8,
                "application/json");
            var result = await client.PostAsync("https://localhost:5020/cancel", requestContent);
            return result.IsSuccessStatusCode;

        }
    }
}
