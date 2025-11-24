using MassTransit;
using Models;

public class MyConsumer : IConsumer<MyMessage>
{
    public Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Received: {context.Message.Content}");
        return Task.CompletedTask;
    }
}