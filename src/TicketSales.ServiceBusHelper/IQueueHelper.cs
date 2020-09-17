using System.Threading.Tasks;

namespace TicketSales.ServiceBusHelper
{
    public interface IQueueHelper
    {
        Task<string> AddNewMessage(string messageBody, string clientId);
        Task<string> GetMessageByCorrelationId(string correlationId, string clientFilter);
        Task<string> SendMessageAwaitReply(string messageBody);
    }
}
