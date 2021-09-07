using Admin.Common;
using Admin.CustomerRead;
using Admin.CustomerUpdate;
using Admin.Extensibility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Admin.App
{
    class Program
    {
        static List<CustomerModel> _customers = new List<CustomerModel>();
        static Random _rnd = new Random();
        static Hook _hook = new Hook();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1 - Read Customer Data");
                Console.WriteLine("2 - Write Customer Data");
                Console.WriteLine("3 - Add Customer");
                Console.WriteLine("0 - Exit");

                var choice = Console.ReadKey();
                switch (choice.Key)
                {
                    case ConsoleKey.D0:
                        return;

                    case ConsoleKey.D1:
                        ReadCustomerData();
                        foreach (var customer in _customers)
                        {
                            Console.WriteLine($"Customer: {customer.Name}");
                        }
                        break;

                    case ConsoleKey.D2:
                        CommitCustomerData();
                        break;

                    case ConsoleKey.D3:
                        ReadCustomerData();
                        _customers.Add(new CustomerModel()
                        {
                            Name = $"Customer {Guid.NewGuid()}",
                            Address = "Customer Address",
                            CreditLimit = _rnd.Next(1000),
                            Email = $"customer{_rnd.Next(10000)}@domain.com"
                        });
                        CommitCustomerData();

                        break;

                }
            }
        }

        private static void ReadCustomerData()
        {
            var read = new ReadService();
            _customers = read.ReadAll(@"c:\tmp\test.txt").ToList();
        }

        private static void CommitCustomerData()
        {
            var write = new WriteService();
            write.Write(_customers, @"c:\tmp\test.txt");

            // Provide hook
            string jsonParams = JsonSerializer.Serialize(_customers);

            _hook.CreateHook(
                methodName: "After",
                className: "CommitCustomerData",
                parameters: new[] { jsonParams });
        }
    }
}
