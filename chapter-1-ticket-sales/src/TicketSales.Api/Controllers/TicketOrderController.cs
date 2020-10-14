using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketSales.Common;
using TicketSales.ThirdPartyProxy;

namespace TicketSales.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TicketOrderController : ControllerBase
    {
        private readonly ILogger<TicketOrderController> _logger;
        private readonly ITicketService _ticketService;                

        public TicketOrderController(ILogger<TicketOrderController> logger,
            ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;                        
        }

        [HttpPost]        
        public async Task<bool> OrderTicket(TicketInformation ticketInformation)
        {
            _logger.Log(LogLevel.Information, "OrderTicket");

            bool isSuccess = await _ticketService.OrderTicket(ticketInformation);
            return isSuccess;
        }
    }
}
