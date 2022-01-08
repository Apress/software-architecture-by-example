using System.Text;
using System.Text.Json;

while (true)
{
    Console.WriteLine("Please select function");
    Console.WriteLine("0 - Exit");
    Console.WriteLine("1 - Check-in");    

    var choice = Console.ReadKey();

    switch (choice.Key)
    {
        case ConsoleKey.D0:
            return;

        case ConsoleKey.D1:
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://ambassador-api");

            var location = new TravelRep.App.Location()
            {
                Latitude = 12,
                Longitude = 256
            };
            Console.WriteLine("Calling checkin API...");
            var content = new StringContent(JsonSerializer.Serialize(location), Encoding.UTF32, "application/json");
            var result = await httpClient.PostAsync("checkin", content);
            result.EnsureSuccessStatusCode();

            Console.WriteLine("Reading results...");
            var results = await result.Content.ReadAsStringAsync();
            Console.WriteLine(results);
            break;
    }
}