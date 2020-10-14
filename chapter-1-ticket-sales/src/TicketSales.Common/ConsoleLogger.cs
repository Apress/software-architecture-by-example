using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSales.Common
{
    public class ConsoleLogger : ILogger
    {
        private readonly ConsoleHelper _consoleHelper;

        public ConsoleLogger(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public void Log(string message)
        {
            _consoleHelper.OutputString(message);
        }

        public void LogError(Exception exception)
        {
            _consoleHelper.OutputWarning(exception.Message);
        }
    }
}
