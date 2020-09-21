using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TicketSales.Common;
using TicketSales.ServiceBusHelper;

namespace TicketSales.MockClient
{
    class Program
    {
        static HttpClient client = new HttpClient();        
        static string _ticketSalesApiPath = "https://localhost:5101/ticketorder";
        static Random _rnd = new Random();
        static ConsoleHelper _consoleHelper;
        static ConsoleLogger _consoleLogger;

        static async Task Main(string[] args)
        {
            _consoleHelper = new ConsoleHelper("Mock Client", ConsoleColor.Cyan);
            _consoleLogger = new ConsoleLogger(_consoleHelper);

            while (true)
            {
                var result = _consoleHelper.GetKeyPress("What would you like to do?",
                    new string[] {
                    "1 - Place Order",
                    "2 - Get Ticket Availability",
                    "3 - Call Order Ticket from Internal API directly"
                });

                switch(result.KeyChar)
                {
                    case '1':
                        await CallServiceBusPlaceOrder(
                            GetServiceBusConfiguration(),
                            _consoleLogger);
                        break;

                    case '2':
                        var tickets = await CallGetTickets();
                        break;

                    case '3':
                        var info = CreateTestTicketInformation();
                        bool orderTicketResult = await CallOrderTicket(info);
                        _consoleLogger.Log($"Order ticket result: {orderTicketResult}");
                        break;
                }
            }
        }

        private static ServiceBusConfiguration GetServiceBusConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .Build();

            return new ServiceBusConfiguration()
            {
                ConnectionString = configuration.GetValue<string>("ServiceBus:ConnectionString"),                
                QueueName = configuration.GetValue<string>("ServiceBus:QueueName"),
                ResponseQueueName = configuration.GetValue<string>("ServiceBus:ResponseQueueName")
            };
        }

        private static async Task CallServiceBusPlaceOrder(
            ServiceBusConfiguration serviceBusConfiguration, ILogger logger)
        {
            var serviceBusHelper = new QueueHelper(
                serviceBusConfiguration, logger);

            var ticketInformation = CreateTestTicketInformation();            

            var messageResult = await serviceBusHelper.SendMessageAwaitReply(
                JsonConvert.SerializeObject(ticketInformation));
            _consoleHelper.OutputString(messageResult);
        }

        private static async Task TestTicketSalesApi()
        {
            _consoleHelper.OutputTime();

            TicketInformation ticketInformation = CreateTestTicketInformation();
            var result = await CallOrderTicket(ticketInformation);

            _consoleHelper.OutputTime();
        }

        private static TicketInformation CreateTestTicketInformation() =>        
            new TicketInformation()
            {
                EventCode = "test",
                EventDate = DateTime.Now.AddDays(30),
                Price = 123,
                Quantity = 1
            };        

        private static async Task<string?> CallGetTickets()
        {            
            HttpResponseMessage response = await client.GetAsync($"{_ticketSalesApiPath}/GetTickets");
            if (response.IsSuccessStatusCode)
            {                                
                var tickets = await GetAllTickets(response);

                foreach (var ticket in tickets)
                {
                    Console.WriteLine($"ticket {ticket.Price}, quantity: {ticket.Quantity}");
                }
            }

            return null;
        }

        private static async Task<bool> CallOrderTicket(TicketInformation ticketInformation)
        {
            var info = new StringContent(
                JsonConvert.SerializeObject(ticketInformation), 
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await client.PostAsync(
                $"{_ticketSalesApiPath}/orderticket", info);
            return (response.IsSuccessStatusCode);
        }

        private static async Task<IEnumerable<TicketInformation>> GetAllTickets(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();       

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };                 
            var result = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TicketInformation>>(data, options);

            return result;
        }

    }
}
