using MassTransit;
using Models;
using producer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) => { 
        cfg.Host("localhost", "/", h => { 
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddScoped<ForcastPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (ForcastPublisher publisher) =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    try
    {
        foreach (var day in forecast)
        {
            var dailyForecast = new DailyWeatherForecast
            {
                Date = day.Date,
                TemperatureC = day.TemperatureC,
                TemperatureF = day.TemperatureF,
                Summary = day.Summary ?? "Nothing"
            };
            
            await publisher.Publish(dailyForecast);
        }

        Console.WriteLine($"INFO: {forecast.Length} прогнозов отправлено");
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR: Ошибка отправки прогнощов {0}", ex.Message);
    }
    
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
