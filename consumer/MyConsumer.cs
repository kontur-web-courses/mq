using MassTransit;
using Models;

namespace consumer;

public class MyConsumer: IConsumer<Model>
{
    public Task Consume(ConsumeContext<Model> context)
    {
        Console.WriteLine($"[{context.Message.Number}] {context.Message.Text}");
        return Task.CompletedTask;
    }
}