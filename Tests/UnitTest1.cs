using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Models;
using Xunit;

namespace Tests;

public class MessagePublishingTests
{
    [Fact]
    public async Task Should_Send_MyMessage_To_Queue()
    {
        using var harness = new InMemoryTestHarness();

        harness.OnConfigureInMemoryBus += bus => bus.ReceiveEndpoint("my-queue", cfg => { });

        await harness.Start();
        try
        {
            var publisher = new MyPublisher(harness.Bus);

            await publisher.Publish("Test message");

            Assert.True(await harness.Sent.Any<MyMessage>());
            var sent = harness.Sent.Select<MyMessage>().First();
            var message = sent.MessageObject as MyMessage;
            Assert.NotNull(message);
            Assert.Equal("Test message", message.Content);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Fact]
    public async Task Should_Send_Correct_Message_Content()
    {
        using var harness = new InMemoryTestHarness();

        harness.OnConfigureInMemoryBus += bus => bus.ReceiveEndpoint("my-queue", cfg => { });

        await harness.Start();
        try
        {
            var publisher = new MyPublisher(harness.Bus);

            await publisher.Publish("Weather forecast requested");

            Assert.True(await harness.Sent.Any<MyMessage>(), "Сообщение не было отправлено");
            var sentMessage = harness.Sent.Select<MyMessage>().First();
            var message = sentMessage.MessageObject as MyMessage;
            Assert.NotNull(message);
            Assert.Equal("Weather forecast requested", message.Content);
        }
        finally
        {
            await harness.Stop();
        }
    }
}
