using MassTransit;
using Shared.Models;

namespace consumer.Brokers.RabbitMq;

public class MyRabbitMqConsumer : IConsumer<NewTaskBrockerRequest>
{
    public Task Consume(ConsumeContext<NewTaskBrockerRequest> context)
    {
        Console.WriteLine($"[{context.Message.MessageId}] {context.Message.Content}");
        return Task.CompletedTask;
    }
}