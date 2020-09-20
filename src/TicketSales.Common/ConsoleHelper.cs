using System;

namespace TicketSales.Common
{
    public class ConsoleHelper
    {
        private readonly string _id;
        private readonly ConsoleColor _consoleColor;

        public ConsoleHelper(string id, ConsoleColor consoleColor)
        {
            _id = id;
            _consoleColor = consoleColor;
        }

        public void OutputTime()
        {
            OutputId();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"Date Stamp: {DateTime.Now}");
            Console.ResetColor();
        }

        private void OutputId()
        {
            Console.ForegroundColor = _consoleColor;
            Console.Write($"({_id}) : ");
        }

        public void OutputString(string message)
        {
            OutputId();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{DateTime.Now}: {message}");
            Console.ResetColor();
        }

        public void OutputWarning(string message)
        {
            OutputId();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now}: {message}");
            Console.ResetColor();
        }

        public void AwaitKeyPress(string prompt = "Please press any key to continue")
        {
            OutputId();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(prompt);
            Console.ReadKey();
            Console.ResetColor();
        }

        public ConsoleKeyInfo GetKeyPress(string mainPrompt, string[] prompts)
        {
            OutputId();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(mainPrompt);

            foreach (string prompt in prompts)
            {
                Console.WriteLine(prompt);
            }
                        
            Console.ResetColor();
            return Console.ReadKey();
        }

    }
}