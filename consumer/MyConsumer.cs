using MassTransit;
using Models;

namespace consumer;

public class MyConsumer : IConsumer<MyMessage>
{
    public Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine(
            $"[{context.Message.MessageId}] {context.Message.Content} RandomInt: {context.Message.RandomInt}");
        return Task.CompletedTask;
    }
}