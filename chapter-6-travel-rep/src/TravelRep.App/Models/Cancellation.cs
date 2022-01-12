using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelRep.App
{
    public record Cancellation
    {
        public string? Report { get; set; }
        public int FlightNumber { get; set; }
    }
}
