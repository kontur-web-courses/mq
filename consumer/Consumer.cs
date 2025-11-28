using MassTransit;
using producer;

namespace consumer;

public class Consumer: IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        Console.WriteLine($"[{context.Message.MessageId}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}