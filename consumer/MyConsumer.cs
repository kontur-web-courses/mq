using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Refer>
{
    public Task Consume(ConsumeContext<Refer> context)
    {
        Console.WriteLine($"[{context.Message.MessageId}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}