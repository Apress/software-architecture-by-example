using Admin.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Admin.CustomerRead
{
    public class ReadService : IReadService
    {
        public CustomerModel Read(string dataFile, string customerName)
        {
            var customers = ReadAllRecords(dataFile);
            return customers.FirstOrDefault(a => a.Name == customerName);
        }

        public IEnumerable<CustomerModel> ReadAll(string dataFile) =>
            ReadAllRecords(dataFile);

        private IEnumerable<CustomerModel> ReadAllRecords(string dataFile)
        {
            string customerData = File.ReadAllText(dataFile);
            var customers = JsonSerializer.Deserialize<IEnumerable<CustomerModel>>(customerData);
            return customers;
        }
    }
}
