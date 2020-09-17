using System;

namespace TicketSales.Common
{
    public static class ConsoleHelper
    {
        public static void OutputTime()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"Date Stamp: {DateTime.Now}");
            Console.ResetColor();
        }

        public static void OutputString(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{DateTime.Now}: {message}");
            Console.ResetColor();
        }

        public static void OutputWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.Now}: {message}");
            Console.ResetColor();
        }

        public static void AwaitKeyPress(string prompt = "Please press any key to continue")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(prompt);
            Console.ReadKey();
            Console.ResetColor();
        }

        public static ConsoleKeyInfo GetKeyPress(string mainPrompt, string[] prompts)
        {
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