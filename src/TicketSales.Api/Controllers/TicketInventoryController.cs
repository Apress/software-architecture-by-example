using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSales.Api;

namespace TicketSales.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketInventoryController
    {
        [HttpGet]
        public IEnumerable<TicketInformation> GetTickets()
        {
            return new List<TicketInformation>()
            {
                new TicketInformation()
                {
                    EventCode = "test"
                }
            };
        }
    }
}
