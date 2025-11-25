using MassTransit;
using Models;

public class MyConsumer: IConsumer<User>
{
    public Task Consume(ConsumeContext<User> context)
    {
        var id = context.Message.Id;
        var content = context.Message.Content;
        Console.WriteLine($"[{id}] {content}");
        
        return Task.CompletedTask;
    }
}