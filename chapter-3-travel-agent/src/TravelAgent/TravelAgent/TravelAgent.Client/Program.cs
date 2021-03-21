using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .AddUserSecrets<Program>()
               .Build();

            string connectionString = configuration.GetValue<string>("ConnectionString");

            while (true)
            {
                Console.WriteLine("Client Simulator: Choose action");
                Console.WriteLine("1 - Make single booking");
                Console.WriteLine("2 - Make 100 bookings");
                Console.WriteLine("3 - Exit");
                var action = Console.ReadKey();

                switch (action.Key)
                {
                    case ConsoleKey.D1:
                        await StartBooking(connectionString);
                        break;

                    case ConsoleKey.D2:
                        for (int i = 1; i <= 100; i++)
                        {
                            await StartBooking(connectionString);
                        }
                        break;

                    case ConsoleKey.D3:
                        return;
                }
                Console.WriteLine("Test complete...");
            }
        }

        private static async Task StartBooking(string connectionString)
        {
            var bookingRequest = new BookingRequest()
            {
                Date = DateTime.Now,
                GuestCount = 1,
                Function = "Book"
            };

            string messageBody = JsonConvert.SerializeObject(bookingRequest);
            await SendMessage(connectionString, messageBody);
        }

        static async Task SendMessage(string connectionString, string messageBody)
        {
            var queueClient = new QueueClient(connectionString, "BookingQueue");

            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }
    }
}
