using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommitCustomerDataExtended
{
    public class CommitCustomerData
    {
        public void After(string parameter)
        {
            Console.WriteLine(parameter);

            using var doc = JsonDocument.Parse(parameter);
            var element = doc.RootElement;

            foreach (var eachElement in element.EnumerateArray())
            {
                string name = eachElement.GetProperty("Name").GetString();
                decimal creditLimit = eachElement.GetProperty("CreditLimit").GetDecimal();

                if (creditLimit > 300)
                {
                    Console.WriteLine($"{name} has a credit limit in excess of £300!");
                }
            }
        }
    }
}
