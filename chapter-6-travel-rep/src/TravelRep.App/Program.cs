using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;
using TravelRep.App;
using TravelRep.App.Models;

var _random = new Random();

var serviceProvider = new ServiceCollection()
    .AddHttpClient()
    .BuildServiceProvider();

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
var systemConfiguration = new SystemConfiguration()
{
    AmbassadorBaseUrl = config["AmbassadorBaseUrl"]
};

while (true)
{
    Console.WriteLine("Please select function");
    Console.WriteLine("0 - Exit");
    Console.WriteLine("1 - Check-in");
    Console.WriteLine("2 - Cancellation");
    Console.WriteLine("3 - Complaint");

    Console.WriteLine("5 - Stress Test");

    var choice = Console.ReadKey();

    switch (choice.Key)
    {
        case ConsoleKey.D0:
            return;

        case ConsoleKey.D1:
            await CallCheckin();
            break;

        case ConsoleKey.D2:
            await CallCancellation();
            break;

        case ConsoleKey.D3:
            await CallComplaint();
            break;

        case ConsoleKey.D5:
            Console.WriteLine("\n\nStarting Stress Test...");
            Parallel.For(0, 1000, async a =>
            {
                switch (_random.Next(2))
                {
                    case 0:
                        await CallCheckin();
                        break;
                    case 1: 
                        await CallComplaint();
                        break;
                    case 2: 
                        await CallComplaint();
                        break;
                }
                
            });
            break;
    }
}

async Task CallCheckin()
{
    var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpFactory.CreateClient();    
    httpClient.BaseAddress = new Uri(systemConfiguration.AmbassadorBaseUrl);

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

async Task CallCancellation()
{
    var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpFactory.CreateClient();    
    httpClient.BaseAddress = new Uri(systemConfiguration.AmbassadorBaseUrl);

    var cancellation = new TravelRep.App.Cancellation()
    {
        Report = "Test report",
        FlightNumber = 116
    };
    Console.WriteLine("\n\nCalling cancellation API...");

    var options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var serialisedContents = JsonSerializer.Serialize(cancellation, typeof(Cancellation), options);

    var content = new StringContent(serialisedContents, Encoding.UTF32, "application/json");
    var result = await httpClient.PostAsync("cancellation", content);

    result.EnsureSuccessStatusCode();

    Console.WriteLine("Reading results...");
    var results = await result.Content.ReadAsStringAsync();
    Console.WriteLine(results);
}

async Task CallComplaint()
{
    var httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpFactory.CreateClient();    
    httpClient.BaseAddress = new Uri(systemConfiguration.AmbassadorBaseUrl);

    var complaint = new Complaint()
    {
        Text = "test complaint text"
    };    
    Console.WriteLine("\n\nCalling complaint API...");
    var content = new StringContent(JsonSerializer.Serialize(complaint), Encoding.UTF32, "application/json");
    var result = await httpClient.PostAsync("complaint", content);
    result.EnsureSuccessStatusCode();

    Console.WriteLine("Reading results...");
    var results = await result.Content.ReadAsStringAsync();
    Console.WriteLine(results);
}
