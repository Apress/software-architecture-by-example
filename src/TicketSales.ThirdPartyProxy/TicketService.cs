using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using TicketSales.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketSales.Common.Models;

namespace TicketSales.ThirdPartyProxy
{
    public class TicketService : ITicketService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TicketServiceConfiguration _ticketServiceConfiguration;

        public TicketService(IHttpClientFactory httpClientFactory,
            TicketServiceConfiguration ticketServiceConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _ticketServiceConfiguration = ticketServiceConfiguration;
        }
        
        public async Task<DataResult<IEnumerable<TicketInformation>>> GetTickets()
        {
            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.GetAsync(
                $"{_ticketServiceConfiguration.Endpoint}/GetTickets");

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };                
                var data = JsonSerializer.Deserialize<IEnumerable<TicketInformation>>(result, options);

                return DataResult<IEnumerable<TicketInformation>>.Success(data);
            }
            else
            {
                // Log error
                return DataResult<IEnumerable<TicketInformation>>.Failure($"Error: {response.ReasonPhrase}");
            }
        }

        public async Task<bool> OrderTicket(TicketInformation ticketInformation)
        {
            var client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client.PostAsync(
                $"{_ticketServiceConfiguration.Endpoint}/PurchaseTicket",
                new StringContent(string.Empty));

            return (response.IsSuccessStatusCode);

        }
    }
}
