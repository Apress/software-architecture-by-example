using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSales.ServiceBusHelper
{
    public class ServiceBusConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
