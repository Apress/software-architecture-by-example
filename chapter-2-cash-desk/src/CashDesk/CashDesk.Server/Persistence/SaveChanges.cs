using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server.Persistence
{
    public class SaveChanges
    {
        public List<object> Changes { get; set; }
        public string StreamName { get; set; }
    }
}
