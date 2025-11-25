using MassTransit.Testing;
using Models;

    
public class MyConsumerTests
{
    [Fact]
    public async Task Consumer_receives_message()
    {
        using var harness = new InMemoryTestHarness();
        var consumerHarness = harness.Consumer<MyConsumer>();

        await harness.Start();
        try
        {
            await harness.InputQueueSendEndpoint.Send(new MyMessage
            {
                MessageId = Guid.NewGuid(),
                Content = "Test message"
            });
            Assert.True(await harness.Sent.Any<MyMessage>());
            Assert.True(await harness.Consumed.Any<MyMessage>());
            Assert.True(await consumerHarness.Consumed.Any<MyMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }
    
    [Fact]
    public async Task Publisher_sends_message_to_bus()
    {
        using var harness = new InMemoryTestHarness();
        await harness.Start();
        try
        {
            var publisher = new MyPublisher(harness.Bus);
            await publisher.Publish();
            
            Assert.True(await harness.Sent.Any<MyMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}

