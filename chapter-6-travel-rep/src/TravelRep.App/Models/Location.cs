using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelRep.App
{
    public record Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
