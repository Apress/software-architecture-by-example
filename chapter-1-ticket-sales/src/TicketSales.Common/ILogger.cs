using System;

namespace TicketSales.Common
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(Exception exception);
    }
}
