using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketSales.Api.Configuration;
using TicketSales.ServiceBusHelper;

namespace TicketSales.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TicketOrderController : ControllerBase
    {
        private readonly ILogger<TicketOrderController> _logger;        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExternalBookingConfiguration _externalBookingConfiguration;

        public TicketOrderController(ILogger<TicketOrderController> logger,
            IHttpClientFactory httpClientFactory,
            ExternalBookingConfiguration externalBookingConfiguration)
        {
            _logger = logger;            
            _httpClientFactory = httpClientFactory;
            _externalBookingConfiguration = externalBookingConfiguration;
        }

        [HttpPost]        
        public async Task<bool> OrderTicket(TicketInformation ticketInformation)
        {
            _logger.Log(LogLevel.Information, "OrderTicket");

            var client = _httpClientFactory.CreateClient();
                
            HttpResponseMessage response = await client.PostAsync(
                $"{_externalBookingConfiguration.ExternalBookingEndpoint}/PurchaseTicket", 
                new StringContent(string.Empty));

            return (response.IsSuccessStatusCode);
        }
    }
}
