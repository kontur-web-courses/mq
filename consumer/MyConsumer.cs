using MassTransit;
using Models;

public class MyConsumer : IConsumer<MyMessage>
{
    public Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine($"[{context.Message.Id}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}
