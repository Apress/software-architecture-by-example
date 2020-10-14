using System;

namespace TicketSales.Service
{
    public class TicketInformation
    {
        public string ClientId { get; set; }

        public string? EventCode { get; set; }

        public DateTime EventDate { get; set; }

        public decimal Price { get; set; }

        public string? SeatCode { get; set; }

        public int Quantity { get; set; }        

    }
}
