using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;

var serviceProvider = new ServiceCollection()
    .AddHttpClient()
    .BuildServiceProvider();

while (true)
{
    Console.WriteLine("Please select function");
    Console.WriteLine("0 - Exit");
    Console.WriteLine("1 - Check-in");  
    
    Console.WriteLine("5 - Stress Test");

    var choice = Console.ReadKey();

    switch (choice.Key)
    {
        case ConsoleKey.D0:
            return;

        case ConsoleKey.D1:
            await CallCheckin();
            break;

        case ConsoleKey.D5:
            Console.WriteLine("\n\nStarting Stress Test...");
            Parallel.For(0, 1000, async a =>
            {
                await CallCheckin();
            });
            break;
    }
}

async Task CallCheckin()
{
    var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpFactory.CreateClient();
    httpClient.BaseAddress = new Uri("http://ambassador-api");

    var location = new TravelRep.App.Location()
    {
        Latitude = 12,
        Longitude = 256
    };
    Console.WriteLine("\n\nCalling checkin API...");
    var content = new StringContent(JsonSerializer.Serialize(location), Encoding.UTF32, "application/json");
    var result = await httpClient.PostAsync("checkin", content);
    result.EnsureSuccessStatusCode();

    Console.WriteLine("Reading results...");
    var results = await result.Content.ReadAsStringAsync();
    Console.WriteLine(results);
}
