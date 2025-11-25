using MassTransit;
using Models;

namespace consumer;
public record OrderSubmitted();

public class MyConsumer : IConsumer<MyMessage>
{
    public async Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine(
            $"[{context.Message.MessageId}] {context.Message.Content} RandomInt: {context.Message.RandomInt}");
        
        await context.RespondAsync(new OrderSubmitted());
    }
}