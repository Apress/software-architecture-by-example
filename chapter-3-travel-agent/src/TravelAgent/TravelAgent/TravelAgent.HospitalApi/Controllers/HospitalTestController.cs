using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAgent.HospitalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HospitalTestController : ControllerBase
    {
        private Random _rnd = new Random();
        private readonly ILogger<HospitalTestController> _logger;

        public HospitalTestController(ILogger<HospitalTestController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/book")]
        public async Task<IActionResult> BookHospitalTest()
        {
            await Task.Delay(_rnd.Next(5000)); // Simulate slow response

            if (_rnd.Next(10) == 1)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost]
        [Route("/cancel")]
        public async Task<IActionResult> CancelHospitalTest()
        {
            await Task.Delay(_rnd.Next(5000)); // Simulate slow response
            return Ok();
        }

    }
}
