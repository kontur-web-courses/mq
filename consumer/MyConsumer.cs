using MassTransit;
using models;

namespace consumer;

public class MyConsumer : IConsumer<InnerInfo>
{
    public Task Consume(ConsumeContext<InnerInfo> context)
    {
        Console.WriteLine($"Подробный прогноз погоды: {context.Message.Info}");
        return Task.CompletedTask;
    }
}