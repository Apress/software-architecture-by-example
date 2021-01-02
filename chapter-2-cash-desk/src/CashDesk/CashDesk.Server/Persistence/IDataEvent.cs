using CashDesk.Server.EventTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server.Persistence
{
    public interface IDataEvent : IEvent
    {
        public string EventType { get; set; }
    }
}
