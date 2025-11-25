using MassTransit.Testing;
using Microsoft.AspNetCore.Routing;
using Models;
using NUnit.Framework;

namespace consumer;

[TestFixture]
public class MyConsumerTest
{
    [Test]
    public async Task ShouldConsumeMessage()
    {
        var harness = new InMemoryTestHarness();
        var consumerHarness = harness.Consumer<MyConsumer>();

        await harness.Start();

        try
        {
            const string content = "Hello";
            await harness.InputQueueSendEndpoint.Send(new MyMessage { MessageId = Guid.NewGuid(), Content = content });

            var message = harness
                .Consumed
                .Select<MyMessage>()
                .FirstOrDefault()!
                .MessageObject as MyMessage;
            var consumedMessage = consumerHarness
                .Consumed
                .Select<MyMessage>()
                .FirstOrDefault()!
                .MessageObject as MyMessage;

            Assert.That(message!.Content, Is.EqualTo(content));
            Assert.That(message.Content, Is.EqualTo(consumedMessage!.Content));
            Assert.That(message.MessageId, Is.EqualTo(consumedMessage.MessageId));
        }
        finally
        {
            await harness.Stop();
        }
    }
}