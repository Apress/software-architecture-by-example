using CashDesk.Server.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server.EventTypes
{
    public class CashDeskTransactionAddedEvent : IEvent
    {
        public ITransaction Transaction { get; set; }

        [JsonIgnore]
        public bool IsNew { get; set; }
    }
}
