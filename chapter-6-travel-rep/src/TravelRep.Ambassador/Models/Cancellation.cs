namespace TravelRep.Ambassador.Models
{
    public record Cancellation
    {
        public string? Report { get; set; }
        public int FlightNumber { get; set; }
    }
}
