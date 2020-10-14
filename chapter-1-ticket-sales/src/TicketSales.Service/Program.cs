using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicketSales.Common;
using TicketSales.ServiceBusHelper;


namespace TicketSales.Service
{
    class Program
    {
        private static ServiceBusConfiguration _serviceBusConfiguration;
        private static string _ticketSalesApiEndpoint;
        private static HttpClient _httpClient;
        private static IQueueHelper _queueHelper;
        private static Guid _clientId = Guid.NewGuid();
        private static ConsoleHelper _consoleHelper;
        private static ConsoleLogger _consoleLogger;

        static void Main(string[] args)
        {
            ReadConfiguration();
            _httpClient = new HttpClient();
            _consoleHelper = new ConsoleHelper("Service", ConsoleColor.Yellow);
            _consoleLogger = new ConsoleLogger(_consoleHelper);
            
            _queueHelper = new QueueHelper(
                _serviceBusConfiguration, 
                _consoleLogger);

            _queueHelper.Listen(onMessageReceived, false);

            _consoleHelper.AwaitKeyPress("Please press any key to exit");
        }

        private static async Task onMessageReceived(Message message, CancellationToken cancellationToken)
        {
            var messageBody = Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);
            _consoleHelper.OutputString($"Correlation Id: {message.CorrelationId} Message: {messageBody}");

            // Deserialise and serialise because these objects are held separately
            var ticketInformation = JsonConvert.DeserializeObject<TicketInformation>(messageBody);

            bool result = await CallOrderTicket(ticketInformation);

            // Put a message back on the queue to indicate the result
            var response = new TicketCreationResponse()
            {
                IsSuccess = result
            };
            
            await _queueHelper.AddNewMessage(
                JsonConvert.SerializeObject(response),                 
                message.CorrelationId);

            await _queueHelper.CompleteReceivedMessage(message);
        }

        private static async Task<bool> CallOrderTicket(TicketInformation ticketInformation)
        {
            var info = new StringContent(
                JsonConvert.SerializeObject(ticketInformation),
                Encoding.UTF8,
                "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{_ticketSalesApiEndpoint}/orderticket", info);
            return (response.IsSuccessStatusCode);
        }

        private static void ReadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddUserSecrets<Program>()
                .Build();

            _serviceBusConfiguration = new ServiceBusConfiguration()
            {
                ConnectionString = configuration.GetValue<string>("ServiceBus:ConnectionString"),
                QueueName = configuration.GetValue<string>("ServiceBus:QueueName"),
                ResponseQueueName = configuration.GetValue<string>("ServiceBus:ResponseQueueName")
            };

            _ticketSalesApiEndpoint = configuration.GetValue<string>("TicketSalesApiEndpoint");
        }
    }
}
