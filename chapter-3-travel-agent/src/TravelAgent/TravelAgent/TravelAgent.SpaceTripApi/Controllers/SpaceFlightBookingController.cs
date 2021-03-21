using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAgent.SpaceTripApi.Models;

namespace TravelAgent.SpaceTripApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpaceFlightBookingController : ControllerBase
    {
        private static Random _rnd = new Random();
        private readonly ILogger<SpaceFlightBookingController> _logger;

        public SpaceFlightBookingController(ILogger<SpaceFlightBookingController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/book")]
        public async Task<IActionResult> BookFlight(BookingRequest bookingRequest)
        {
            await Task.Delay(_rnd.Next(5000)); // Simulate slow response

            if (_rnd.Next(200) == 1)
            {
                return NotFound("Unable to book space flight");
            }
            return Ok();
        }

    }
}
