using Microsoft.Azure.ServiceBus;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicketSales.Common;

namespace TicketSales.ServiceBusHelper
{
    public class QueueHelper : IQueueHelper, IAsyncDisposable
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IQueueClient _queueClient;

        private readonly ILogger _logger;

        public QueueHelper(ServiceBusConfiguration serviceBusConfiguration,
            ILogger logger)
        {
            _connectionString = serviceBusConfiguration.ConnectionString;
            _queueName = serviceBusConfiguration.QueueName;
            _queueClient = new QueueClient(_connectionString, _queueName);
            _logger = logger;
        }

        public async Task<string> AddNewMessage(string messageBody, string clientId)
        {            
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                CorrelationId = Guid.NewGuid().ToString(),
                ReplyTo = clientId
            };
                       
            await _queueClient.SendAsync(message);

            _logger.Log($"AddNewMessage: {message.CorrelationId}, Client: {clientId}");

            return message.CorrelationId;
        }

        public async Task<string> SendMessageAwaitReply(string messageBody)
        {
            string clientId = Guid.NewGuid().ToString();

            var correlationId = await AddNewMessage(messageBody, clientId);
            var result = await GetMessageByCorrelationId(correlationId, clientId);

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            await _queueClient.CloseAsync();
        }

        public async Task<string> GetMessageByCorrelationId(string correlationId, string clientFilter)
        {
            _logger.Log($"GetMessageByCorrelationId: {correlationId}");

            var tcs = new TaskCompletionSource<Message>();
            string returnMessageBody = string.Empty;

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false                
            };

            _queueClient.RegisterMessageHandler(async (message, cancellationToken) =>
            {
                _logger.Log($"Received Message: {message.CorrelationId}, To: {message.To}, ReplyTo: {message.ReplyTo}");
                if (message.CorrelationId == correlationId && message.To == clientFilter)
                {                    
                    returnMessageBody = Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);

                    _logger.Log($"Received Message: {returnMessageBody}, Client: {clientFilter}");

                    tcs.TrySetResult(message);
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                } 
                else
                { 
                    tcs.TrySetResult(null);
                    await _queueClient.AbandonAsync(message.SystemProperties.LockToken);                  
                }
            }, messageHandlerOptions);

            await tcs.Task;
            return returnMessageBody;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception);
            return Task.CompletedTask;
        }

        public void Listen(
            Func<Message, CancellationToken, Task> onMessageReceived,
            bool autoComplete)
        {
            _logger.Log("TicketSales.Service.Listen");
            
            string returnMessageBody = string.Empty;

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = autoComplete
            };

            _queueClient.RegisterMessageHandler(onMessageReceived, messageHandlerOptions);
        }

    }
}
