using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Notification>
{
    public Task Consume(ConsumeContext<Notification> context)
    {
        Console.WriteLine($"[{context.Message.Id}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}