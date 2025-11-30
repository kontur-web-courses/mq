using MassTransit;
using Models;

namespace consumer;

public class MyConsumer : IConsumer<PublishMessage>
{
    public Task Consume(ConsumeContext<PublishMessage> context)
    {
        var message = context.Message;
        Console.WriteLine(
            $"Received new message. Message parameters: id: {message.MessageId}, amount: {message.Amount}, customer: {message.CustomerName}"
        );
        return Task.CompletedTask;
    }
}