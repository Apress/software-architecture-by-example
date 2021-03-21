using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Coordinator
{
    public class HospitalProxy
    {
        private IHttpClientFactory _httpClientFactory;

        public HospitalProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CallHospitalApi(DateTime date) =>        
            await BookHospitalRoom(date);

        private async Task<bool> BookHospitalRoom(DateTime date)
        {
            var client = _httpClientFactory.CreateClient();            

            var requestContent = new StringContent(@"{ 'RoomRequest': { 'Date': '" + date.ToString("yyyy-MM-dd") + "' } }");

            var result = await client.PostAsync("https://localhost:5001/book", requestContent);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> CancelHospitalBooking(DateTime date)
        {
            var client = _httpClientFactory.CreateClient();

            var requestContent = new StringContent(@"{ 'CancellationRequest': { 'Date': '" + date.ToString("yyyy-MM-dd") + "' } }");

            var result = await client.PostAsync("https://localhost:5001/cancel", requestContent);
            return result.IsSuccessStatusCode;

        }
    }
}
