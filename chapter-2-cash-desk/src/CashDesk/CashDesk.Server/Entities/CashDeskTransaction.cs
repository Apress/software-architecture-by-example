using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server.Entities
{
    public class CashDeskTransaction : ITransaction
    {
        public CashDeskTransaction(DateTime dateTime, string productCode, string description, decimal amount, string cashDeskId)
        {
            DateStamp = dateTime;
            ProductCode = productCode;
            Description = description;
            Amount = amount;
            CashDeskId = cashDeskId;
        }

        public DateTime DateStamp { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string CashDeskId { get; set; }
    }
}
