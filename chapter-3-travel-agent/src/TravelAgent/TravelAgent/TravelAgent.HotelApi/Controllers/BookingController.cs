using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAgent.HotelApi.Models;

namespace TravelAgent.HotelApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private static Random _rnd = new Random();
        private readonly ILogger<BookingController> _logger;

        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/book")]
        public async Task<IActionResult> BookRoom(RoomRequest roomRequest)
        {
            await Task.Delay(_rnd.Next(5000)); // Simulate slow response

            if (_rnd.Next(30) == 1)
            {
                return NotFound("No available rooms");
            }
            return Ok();
        }

        [HttpPost]
        [Route("/cancel")]
        public async Task<IActionResult> CancelRoom(RoomRequest roomRequest)
        {
            await Task.Delay(_rnd.Next(5000)); // Simulate slow response

            if (_rnd.Next(30) == 1)
            {
                return BadRequest("Cancellation refused");
            }
            return Ok();
        }

    }
}
