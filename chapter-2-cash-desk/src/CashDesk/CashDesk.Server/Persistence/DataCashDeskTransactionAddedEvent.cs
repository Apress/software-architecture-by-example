using CashDesk.Server.EventTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server.Persistence
{
    public class DataCashDeskTransactionAddedEvent : CashDeskTransactionAddedEvent
    {
        public DataCashDeskTransactionAddedEvent() { }

        public DataCashDeskTransactionAddedEvent(string eventType)
        {
            EventType = eventType;
        }

        public string EventType { get; set; }
    }
}
