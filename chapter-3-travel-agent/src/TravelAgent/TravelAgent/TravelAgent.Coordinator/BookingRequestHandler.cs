using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Client;

namespace TravelAgent.Coordinator
{
    public class BookingRequestHandler : IBookingRequestHandler
    {
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LinkedList<string> _endPoints;

        private readonly string BOOKING_REQUEST = "BookingRequest";
        private readonly string BOOK_HOSPITAL = "BookHospital";
        private readonly string BOOK_HOTEL = "BookHotel";
        private readonly string BOOK_SPACEFLIGHT = "BookSpaceFlight";
        private readonly string BOOKING_COMPLETED = "BookingCompleted";

        private readonly string FUNCTION_BOOK = "Book";
        private readonly string FUNCTION_CANCEL = "Cancel";

        public BookingRequestHandler(string connectionString, IHttpClientFactory httpClientFactory)
        {
            _connectionString = connectionString;
            _httpClientFactory = httpClientFactory;

            _endPoints = new LinkedList<string>();
            _endPoints.AddLast(BOOKING_REQUEST);
            _endPoints.AddLast(BOOK_HOSPITAL);
            _endPoints.AddLast(BOOK_HOTEL);
            _endPoints.AddLast(BOOK_SPACEFLIGHT);
            _endPoints.AddLast(BOOKING_COMPLETED);
        }

        public async Task ProcessBookingRequest(string type, DateTime date, string function)
        {            
            var node = _endPoints.Find(type);

            switch (type)
            {
                case "BookingRequest":
                    if (function == FUNCTION_BOOK)
                    {
                        Console.WriteLine("Booking Request");
                        await SendMessage(_connectionString, node.Next.Value, date, function);
                    }
                    else if (function == FUNCTION_CANCEL)
                    {
                        Console.WriteLine("Booking Request Failed and Successfully Cancelled");
                    }
                    break;

                case "BookHospital":
                    await CallBookHospital(date, function, node);

                    break;

                case "BookHotel":
                    await CallBookHotel(date, function, node);

                    break;

                case "BookSpaceFlight":
                    await CallBookSpaceFlight(date, function, node);

                    break;
            }

        }

        private async Task CallBookHospital(DateTime date, string function, LinkedListNode<string> node)
        {
            var hospitalProxy = new HospitalProxy(_httpClientFactory);

            if (function == FUNCTION_BOOK)
            {
                if (await hospitalProxy.CallHospitalApi(date))
                {
                    Console.WriteLine("Successfully booked hospital room");
                    await SendMessage(_connectionString, node.Next.Value, date, function);
                }
                else
                {
                    Console.WriteLine("Unable to book hospital room.  Cancelling");
                    await SendMessage(_connectionString, node.Previous.Value, date, FUNCTION_CANCEL);
                }
            }
            else if (function == FUNCTION_CANCEL)
            {
                if (await hospitalProxy.CancelHospitalBooking(date))
                {
                    Console.WriteLine("Successfully cancelled hospital room");
                }
                else
                {
                    Console.WriteLine("Unable to cancel hospital room");
                }
                await SendMessage(_connectionString, node.Previous.Value, date, function);
            }
        }

        private async Task CallBookHotel(DateTime date, string function, LinkedListNode<string> node)
        {
            var hotelProxy = new HotelProxy(_httpClientFactory);
            if (function == FUNCTION_BOOK)
            {
                if (await hotelProxy.CallHotelApi(date))
                {
                    Console.WriteLine("Successfully booked hotel room");
                    await SendMessage(_connectionString, node.Next.Value, date, function);
                }
                else
                {
                    Console.WriteLine("Unable to book hotel room.  Cancelling...");
                    await SendMessage(_connectionString, node.Previous.Value, date, FUNCTION_CANCEL);
                }
            }
            else if (function == FUNCTION_CANCEL)
            {
                if (await hotelProxy.CancelHotelRoom(date))
                {
                    Console.WriteLine("Successfully cancelled hotel room");
                }
                else
                {
                    Console.WriteLine("Unable to cancel hotel room");
                }
                await SendMessage(_connectionString, node.Previous.Value, date, function);
            }
        }

        private async Task CallBookSpaceFlight(DateTime date, string function, LinkedListNode<string> node)
        {
            var spaceflightProxy = new SpaceFlightProxy(_httpClientFactory);
            if (function == FUNCTION_BOOK)
            {
                if (await spaceflightProxy.CallSpaceFlightApi(date))
                {
                    Console.WriteLine("Successfully booked space flight");
                    await SendMessage(_connectionString, node.Next.Value, date, function);
                }
                else
                {
                    Console.WriteLine("Unable to book space flight.  Cancelling...");
                    await SendMessage(_connectionString, node.Previous.Value, date, FUNCTION_CANCEL);
                }
            }
            else
            {
                throw new Exception("Cannot cancel space flight");
            }
        }

        private async Task SendMessage(string connectionString, string type, DateTime date, string function)
        {
            var bookingRequest = new BookingRequest()
            {
                Date = date,
                Type = type,
                Function = function
            };

            var queueClient = new QueueClient(connectionString, "BookingQueue");

            string messageBody = JsonConvert.SerializeObject(bookingRequest);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
            await queueClient.CloseAsync();
        }

    }
}
