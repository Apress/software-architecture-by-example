using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketSales.Common;
using TicketSales.ThirdPartyProxy;

namespace TicketSales.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketInventoryController
    {
        private readonly ITicketService _ticketService;

        public TicketInventoryController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IEnumerable<TicketInformation>> GetTickets()
        {
            var result = await _ticketService.GetTickets();
            if (result.IsSuccess)
            {
                return result.Data;
            }
            else
            {
                // Log Error
                return null;
            }
        }
    }
}
