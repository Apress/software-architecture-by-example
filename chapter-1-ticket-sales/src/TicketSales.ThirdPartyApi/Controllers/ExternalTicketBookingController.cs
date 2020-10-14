using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TicketSales.ThirdPartyApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExternalTicketBookingController : ControllerBase
    {
        private static Random _random = new Random();
        private static int _capacity = 1200000;
        private static string _eventCode = "GLS_21";
        private static DateTime _eventDate = new DateTime(2021, 06, 23);
        private static decimal[] _prices = new [] {
            100.50m, 260.65m, 540.10m
        };

        [HttpGet]                
        public IEnumerable<TicketInformation> GetTickets()
        {            
            var tickets = new List<TicketInformation>();

            for (int i = 0; i < _prices.Length; i++)
            {                
                tickets.Add(new TicketInformation()
                {
                    EventCode = _eventCode,
                    EventDate = _eventDate,
                    Price = _prices[_random.Next(_prices.Length)],
                    SeatCode = "NA",
                    Quantity = _capacity / _prices.Length
                });
            }

            return tickets;
        }

        [HttpPost("{seatCode?}")]
        public async Task<IActionResult> ReserveTicket(string seatCode)
        {
            await Task.Delay(1000);

            if (_random.Next(10) == 1)
            {
                return BadRequest();
            }
            return Ok();
        }
        
        [HttpPost("{seatCode?}")]
        public async Task<IActionResult> PurchaseTicket(string seatCode)
        {
            await Task.Delay(2000);

            if (_random.Next(10) == 1)
            {
                return BadRequest();
            }
            return Ok();            
        }
    }
}
