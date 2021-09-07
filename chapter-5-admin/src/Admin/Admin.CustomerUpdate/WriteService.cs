using Admin.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Admin.CustomerUpdate
{
    public class WriteService : IWriteService
    {
        public void Write(IEnumerable<CustomerModel> customers, string file)
        {
            string serialisedCustomers = JsonSerializer.Serialize(customers);
            File.WriteAllText(file, serialisedCustomers);
        }
    }
}
