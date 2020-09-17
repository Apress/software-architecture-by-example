using System;

namespace TicketSales.ThirdPartyApi
{
    public class TicketInformation
    {
        public string EventCode { get; set; }

        public DateTime EventDate { get; set; }

        public decimal Price { get; set; }

        public string SeatCode { get; set; }

        public int Quantity { get; set; }

    }
}
