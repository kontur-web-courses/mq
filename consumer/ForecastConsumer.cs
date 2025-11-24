using MassTransit;
using Models;

namespace consumer;

public class ForecastConsumer : IConsumer<DailyWeatherForecast>
{
    public Task Consume(ConsumeContext<DailyWeatherForecast> context)
    {
        Console.WriteLine("CONSUMER");
        Console.WriteLine($"[{context.Message.Date}] temp={context.Message.TemperatureC} | {context.Message.TemperatureF} deg\nsummary={context.Message.Summary}");
        return Task.CompletedTask;
    }
}