using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models.Ticker;
using TickerQCronTest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<MyJob>();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlite("Data Source=app.db");
});
builder.Services.AddTickerQ(options =>
{
    options.AddOperationalStore<MyDbContext>(efOpt =>
    {
        efOpt.UseModelCustomizerForMigrations(); // Applies custom model customization only during EF Core migrations
        efOpt.CancelMissedTickersOnApplicationRestart(); // Useful in distributed mode
    }); // Enables EF-backed storage
    options.AddDashboard(basePath: "/tickerq-dashboard"); // Dashboard path
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/crontest", async (MyJob myJob) =>
{
    var result1 = await myJob.QueueCron("*/1 * * * *"); // Every minute
    var result2 = await myJob.QueueCron("0 0 * * *"); // Daily at midnight
    return Results.Ok(new {
        First = result1.IsSucceded,
        Second = result2.IsSucceded
    });
})
.WithName("CronTest");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
