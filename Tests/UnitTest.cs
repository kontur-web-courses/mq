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

            await publisher.Publish("msg");

            Assert.True(await harness.Sent.Any<Message>());
            var sent = harness.Sent.Select<Message>().First();
            var message = sent.MessageObject as Message;
            Assert.NotNull(message);
            Assert.Equal("msg", message.Text);
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

            await publisher.Publish("msg1");

            Assert.True(await harness.Sent.Any<Message>(), "Сообщение не было отправлено");
            var sentMessage = harness.Sent.Select<Message>().First();
            var message = sentMessage.MessageObject as Message;
            Assert.NotNull(message);
            Assert.Equal("msg1", message.Text);
        }
        finally
        {
            await harness.Stop();
        }
    }
}