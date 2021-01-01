using CashDesk.Server.Entities;
using CashDesk.Server.EventTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.Server
{
    public class CashDeskEventStream : EventStreamBase
    {
        public Dictionary<string, decimal> CashDeskBalance { get; set; } =
            new Dictionary<string, decimal>();

        public void AddTransaction(CashDeskTransaction transaction)
        {                        

            var trans = new CashDeskTransactionAddedEvent()
            {
                Transaction = transaction,
                IsNew = true
            };
            Apply(trans);
        }

        protected override void When(object theEvent)
        {
            switch (theEvent)
            {
                case CashDeskTransactionAddedEvent transactionEvent:
                    var transaction = (CashDeskTransaction)transactionEvent.Transaction;

                    if (!CashDeskBalance.ContainsKey(transaction.CashDeskId))
                        CashDeskBalance[transaction.CashDeskId] = 0;

                    CashDeskBalance[transaction.CashDeskId] += transaction.Amount;
                    break;
            }
        }
    }
}
