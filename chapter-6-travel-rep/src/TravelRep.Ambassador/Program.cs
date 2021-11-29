using Hangfire;
using Hangfire.LiteDB;
using TravelRep.Ambassador;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddHangfire(configuration =>
{
    configuration.UseLiteDbStorage("./hf.db");
});
builder.Services.AddHangfireServer();

var config = new SystemConfiguration()
{
    CentralSystem = builder.Configuration["CentralSystemUrl"]
};
builder.Services.AddSingleton<SystemConfiguration>(config);

builder.Services.AddSingleton<ICentralSystemProxyService, CentralSystemProxyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/checkin", async (Location location, ICentralSystemProxyService centralSystemProxyService, IHttpClientFactory httpClientFactory) =>
{    
    var result = await centralSystemProxyService.CallCheckin(location.Longitude, location.Latitude);
    if (result) return Results.Ok();
    return Results.Accepted();
});

app.Run();
