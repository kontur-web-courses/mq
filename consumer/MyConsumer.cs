using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Model>
{
    public Task Consume(ConsumeContext<Model> context)
    {
        Console.WriteLine($"[{context.Message.Id}] {context.Message.Message}");
        return Task.CompletedTask;
    }
}