using System;
using System.Collections.Generic;
using System.Text;

namespace TravelAgent.Client
{
    public class BookingRequest
    {
        public string Type { get; set; } = "BookingRequest";
        public DateTime Date { get; set; }
        public int GuestCount { get; set; }
        public string Function { get; set; }
    }
}
