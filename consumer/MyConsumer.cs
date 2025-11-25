using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        Console.WriteLine($"[{context.Message.MessageId}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}