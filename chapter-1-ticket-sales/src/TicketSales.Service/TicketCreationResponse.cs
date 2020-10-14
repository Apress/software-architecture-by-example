using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSales.Service
{
    public class TicketCreationResponse
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
    }
}
