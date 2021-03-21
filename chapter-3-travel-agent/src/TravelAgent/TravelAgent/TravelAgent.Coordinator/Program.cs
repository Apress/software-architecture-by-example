using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TravelAgent.Coordinator
{
    class Program
    {
        private static IHttpClientFactory _httpClientFactory;
        private static IBookingRequestHandler _bookingRequestHandler;
        private static QueueClient _queueClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Transaction Coordinator: Start Process - press any key");
            Console.ReadKey();            

            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .AddUserSecrets<Program>()
               .Build();

            string connectionString = configuration.GetValue<string>("ConnectionString");

            _queueClient = new QueueClient(connectionString, "BookingQueue");

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionHandler);
            messageHandlerOptions.AutoComplete = false;

            _queueClient.RegisterMessageHandler(handleMessage, messageHandlerOptions);            

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IBookingRequestHandler, BookingRequestHandler>(srv =>
                {
                    return new BookingRequestHandler(connectionString, srv.GetService<IHttpClientFactory>());
                })
                .AddHttpClient()
                .BuildServiceProvider();            
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            _bookingRequestHandler = serviceProvider.GetService<IBookingRequestHandler>();

            Console.WriteLine("Coordinator started...");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine("Something bad happened!");
            return Task.CompletedTask;
        }

        private static async Task handleMessage(Message message, CancellationToken cancellation)
        {
            string messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("Message received: {0}", messageBody);

            var json = JObject.Parse(messageBody);
            string type = json.SelectToken("Type").Value<string>();
            DateTime date = json.SelectToken("Date").Value<DateTime>();
            string function = json.SelectToken("Function").Value<string>();

            await _bookingRequestHandler.ProcessBookingRequest(type, date, function);
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
