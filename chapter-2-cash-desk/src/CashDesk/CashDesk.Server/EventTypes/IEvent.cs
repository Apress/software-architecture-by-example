using CashDesk.Server.Entities;

namespace CashDesk.Server.EventTypes
{
    public interface IEvent
    {
        ITransaction Transaction { get; set; }
        bool IsNew { get; set; }
    }
}