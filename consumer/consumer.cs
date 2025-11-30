using MassTransit;
using Models;

public class MyConsumer: IConsumer<User>
{
    public Task Consume(ConsumeContext<User> context)
    {
        Console.WriteLine($"[{context.Message.UserId}] {context.Message.Name} {context.Message.Age}");
        return Task.CompletedTask;
    }
}