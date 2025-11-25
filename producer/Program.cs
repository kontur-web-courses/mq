using System.Buffers;
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
builder.Services.AddScoped<MyPublisher>();

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

app.MapGet("/weatherforecast", async (s) =>
    {
        var publisher = s.RequestServices.GetRequiredService<MyPublisher>();
        await publisher.Publish("querySSr");
        var forecast =  Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        var result = forecast.ToString().Select(b => (byte)b).ToArray().AsSpan();
        s.Response.ContentType = "application/json";
        s.Response.StatusCode = 200;
        s.Response.ContentLength = result.Length;
        s.Response.BodyWriter.Write(result);
    // return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
