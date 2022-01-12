using Hangfire;
using Hangfire.Dashboard;
using Hangfire.LiteDB;
using Microsoft.AspNetCore.Mvc;
using TravelRep.Ambassador;
using TravelRep.Ambassador.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddLogging();

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

var options = new DashboardOptions()
{
    Authorization = new[] { new AuthorizationFilter() }
};
app.UseHangfireDashboard("/hangfire", options);

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.MapPost("/checkin", async ([FromBody]Location location, ICentralSystemProxyService centralSystemProxyService, IHttpClientFactory httpClientFactory) =>
{    
    var result = await centralSystemProxyService.CallCheckin(location.Longitude, location.Latitude);
    if (result) return Results.Ok();
    return Results.Accepted();
});
app.MapPost("/cancellation", async ([FromBody]Cancellation cancellation, ICentralSystemProxyService centralSystemProxyService, IHttpClientFactory httpClientFactory) =>
{    
    if (string.IsNullOrWhiteSpace(cancellation.Report)) return Results.BadRequest(cancellation);   

    var result = await centralSystemProxyService.CallCancellation(cancellation.Report, cancellation.FlightNumber);
    if (result) return Results.Ok();
    return Results.Accepted();
});
app.MapPost("/complaint", async ([FromBody] Complaint complaint, ICentralSystemProxyService centralSystemProxyService, IHttpClientFactory httpClientFactory) =>
{
    if (string.IsNullOrWhiteSpace(complaint.Text)) return Results.BadRequest(complaint);

    var result = await centralSystemProxyService.CallComplaint(complaint.Text);
    if (result) return Results.Ok();
    return Results.Accepted();
});

app.Run();

public class AuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}