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
    public class AvailabilityController : ControllerBase
    {
        private static Random _rnd = new Random();
        private readonly ILogger<AvailabilityController> _logger;

        public AvailabilityController(ILogger<AvailabilityController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get(DateTime date)
        {         
            var response = new AvailabilityResponse()
            {
                Price = _rnd.Next(1) == 0 ? 120 : 150,
                RoomCount = _rnd.Next(3)
            };

            return Ok(response);
        }
    }
}
