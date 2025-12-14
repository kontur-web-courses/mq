using MassTransit;
using Models;

public class MyConsumer: IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        Console.WriteLine($"[{context.Message.Id}] {context.Message.Text}");
        return Task.CompletedTask;
    }
}