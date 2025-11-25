using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Class1>
{
    public Task Consume(ConsumeContext<Class1> context)
    {
        
        Console.WriteLine($"[{context.MessageId}] {context.Message.message}");
        return Task.CompletedTask;
    }
}