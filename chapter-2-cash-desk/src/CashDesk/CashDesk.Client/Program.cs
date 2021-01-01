using CashDesk.DataPersistence;
using CashDesk.Server;
using CashDesk.Server.Entities;
using CashDesk.Server.EventTypes;
using CashDesk.Server.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace CashDesk.Client
{
    class Program
    {
        private static DataEventsPersistence _dataEvents = new DataEventsPersistence();
        private static PersistData<CashDeskEventStream> _persistData = new PersistData<CashDeskEventStream>(_dataEvents);
        private static CashDeskEventStream _cashDeskEventStream;

        static void Main(string[] args)
        {
            _cashDeskEventStream = _persistData.Load("Stream1");

            while (true)
            {
                Console.WriteLine("1 - Add till transaction");
                Console.WriteLine("2 - View till balance");
                Console.WriteLine("3 - View all till balances");
                Console.WriteLine("4 - View all transactions");
                Console.WriteLine("5 - Exit");

                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        AddTransactionDialog();
                        break;

                    case ConsoleKey.D2:
                        GetBalanceForTillDialog();
                        break;

                    case ConsoleKey.D3:
                        GetBalanceForAllTillsDialog();
                        break;

                    case ConsoleKey.D4:
                        GetTransactionHistory();
                        break;

                    default:
                        _persistData.Save(_cashDeskEventStream);
                        return;
                }
            }            
        }

        private static void GetTransactionHistory()
        {            
            foreach (var eachEvent in _cashDeskEventStream.Changes)
            {
                var cashDeskTransactionAddedEvent = (CashDeskTransactionAddedEvent)eachEvent;
                var transaction = (CashDeskTransaction)cashDeskTransactionAddedEvent.Transaction;

                Console.WriteLine($"{transaction.DateStamp}: {transaction.CashDeskId}" +
                    $" - {transaction.ProductCode} ({transaction.Amount})");
            }

        }

        private static void GetBalanceForAllTillsDialog()
        {
            decimal balanceTotal = _cashDeskEventStream.CashDeskBalance.Sum(a => a.Value);

            Console.WriteLine($"Balance across the whole estate is {balanceTotal}");
        }
       
        private static void GetBalanceForTillDialog()
        {
            Console.Write("Cash Desk Id: ");
            string cashDeskId = Console.ReadLine();

            decimal balance = GetBalanceFor(cashDeskId);
            Console.WriteLine($"Balance for {cashDeskId} is {balance.ToString()}");
        }

        private static void AddTransactionDialog()
        {
            Console.Write("Cash Desk Id: ");
            string cashDeskId = Console.ReadLine();

            Console.Write("Product Code: ");
            string productCode = Console.ReadLine();

            Console.Write("Description: ");
            string description = Console.ReadLine();

            Console.Write("Amount: ");
            string amount = Console.ReadLine();

            AddTransaction(productCode, description, amount, cashDeskId);            
        }

        private static decimal GetBalanceFor(string cashDeskId) =>
            _cashDeskEventStream.CashDeskBalance[cashDeskId];        

        static void AddTransaction(string productCode, string description,
            string amountString, string cashDeskId)
        {
            decimal amount = Decimal.Parse(amountString);

            var transaction = new CashDeskTransaction(DateTime.Now,
                productCode, description, amount, cashDeskId);

            _cashDeskEventStream.AddTransaction(transaction);
        }
    }
}
