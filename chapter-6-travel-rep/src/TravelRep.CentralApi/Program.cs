var builder = WebApplication.CreateBuilder(args);

Random _random = new Random();

async Task ChaosMonkey()
{
    Console.WriteLine($"\n\nChaosMonkey Invoked: {DateTime.Now}");

    int result = _random.Next(5);
    switch (result)
    {
        case 0:
            Console.WriteLine($"Throw exception immediately");
            throw new Exception("Failure");

        case 1:
            Console.WriteLine($"Wait, then throw exception");
            await Task.Delay(3000);
            throw new Exception("Failure");

        case 2:
            Console.WriteLine($"Wait, then work");
            await Task.Delay(3000);
            break;
    }
    Console.WriteLine("Call succeeded");
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/checkin", async (double longitude, double latitude) =>
{
    Console.Write($"\n\n/checkin called: {DateTime.Now}\n\n\n");

    await ChaosMonkey();
    return Results.Ok();
});

app.MapPost("/cancellation", async (string report, int flightNumber) =>
{
    await ChaosMonkey();
    return Results.Ok();
});

app.MapPost("/complaint", async (string complaint) =>
{
    await ChaosMonkey();
    return Results.Ok();
});


app.Run();
