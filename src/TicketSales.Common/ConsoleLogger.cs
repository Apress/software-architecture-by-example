using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSales.Common
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            ConsoleHelper.OutputString(message);
        }

        public void LogError(Exception exception)
        {
            ConsoleHelper.OutputWarning(exception.Message);
        }
    }
}
