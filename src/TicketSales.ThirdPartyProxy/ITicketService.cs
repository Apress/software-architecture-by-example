using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TicketSales.Common;
using TicketSales.Common.Models;

namespace TicketSales.ThirdPartyProxy
{
    public interface ITicketService
    {
        Task<DataResult<IEnumerable<TicketInformation>>> GetTickets();
        Task<bool> OrderTicket(TicketInformation ticketInformation);
    }
}
