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
        private readonly IQueueClient _responseQueueClient;
        private readonly IQueueClient _sendQueueClient;

        private readonly ILogger _logger;

        public QueueHelper(ServiceBusConfiguration serviceBusConfiguration,
            ILogger logger)
        {                        
            _sendQueueClient = new QueueClient(
                serviceBusConfiguration.ConnectionString, 
                serviceBusConfiguration.QueueName,
                ReceiveMode.PeekLock);
            _responseQueueClient = new QueueClient(
                serviceBusConfiguration.ConnectionString, 
                serviceBusConfiguration.ResponseQueueName, 
                ReceiveMode.PeekLock);
            _logger = logger;
        }

        public async Task<string> AddNewMessage(string messageBody, string correlationId = "")
        {            
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId,                
            };
                       
            await _sendQueueClient.SendAsync(message);

            _logger.Log($"AddNewMessage: {message.CorrelationId}");

            return message.CorrelationId;
        }

        public async Task<string> AddResponseMessage(string messageBody, string correlationId = "")
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId                
            };

            await _responseQueueClient.SendAsync(message);

            _logger.Log($"AddResponseMessage: {message.CorrelationId}");

            return message.CorrelationId;
        }

        public async Task<string> SendMessageAwaitReply(string messageBody)
        {
            var correlationId = await AddNewMessage(messageBody);
            var result = await GetMessageByCorrelationId(correlationId);

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            await _sendQueueClient.CloseAsync();
        }

        public async Task<string> GetMessageByCorrelationId(string correlationId)
        {
            _logger.Log($"GetMessageByCorrelationId: {correlationId}");

            var tcs = new TaskCompletionSource<Message>();
            string returnMessageBody = string.Empty;

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false                
            };

            _responseQueueClient.RegisterMessageHandler(async (message, cancellationToken) =>
            {
                _logger.Log($"Received Message: {message.CorrelationId}, To: {message.To}, ReplyTo: {message.ReplyTo}");
                if (message.CorrelationId == correlationId)
                {                    
                    returnMessageBody = Encoding.UTF8.GetString(message.Body, 0, message.Body.Length);                    
                    
                    await _responseQueueClient.CompleteAsync(message.SystemProperties.LockToken);
                    _logger.Log($"Accepted Message: {returnMessageBody}");

                    tcs.TrySetResult(message);
                } 
                else
                {
                    await _responseQueueClient.AbandonAsync(message.SystemProperties.LockToken);
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

            _sendQueueClient.RegisterMessageHandler(onMessageReceived, messageHandlerOptions);
        }

        public async Task CompleteReceivedMessage(Message message)
        {
            await _sendQueueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
