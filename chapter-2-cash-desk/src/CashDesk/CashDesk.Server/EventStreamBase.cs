using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CashDesk.Server
{
    public abstract class EventStreamBase
    {
        public string StreamName { get; set; }
        public List<object> Changes = new List<object>();        

        public void Apply(object theEvent)
        {
            When(theEvent);
            Changes.Add(theEvent);
        }

        protected abstract void When(object theEvent);
    }
}
