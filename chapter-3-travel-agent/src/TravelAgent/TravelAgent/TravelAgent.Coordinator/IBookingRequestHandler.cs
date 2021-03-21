using System;
using System.Threading.Tasks;

namespace TravelAgent.Coordinator
{
    public interface IBookingRequestHandler
    {
        Task ProcessBookingRequest(string requestType, DateTime date, string function);
        
    }
}