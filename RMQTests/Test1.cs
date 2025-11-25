using consumer;
using MassTransit;
using MassTransit.Testing;
using Models;
using producer;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace RMQTests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public async Task Should_Consume2()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<MyConsumer>(); })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();

        await harness.Start();

        var client = harness.GetRequestClient<MyMessage>();

        await client.GetResponse<OrderSubmitted>(new MyMessage
        {
            MessageId = InVar.Id,
            Content = "123"
        });

        Assert.IsTrue(await harness.Consumed.Any<MyMessage>());

        var consumerHarness = harness.GetConsumerHarness<MyConsumer>();

        Assert.IsTrue(await consumerHarness.Consumed.Any<MyMessage>());
    }

    [TestMethod]
    public async Task Should_Consume()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<MyConsumer>(); })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await harness.Bus.Publish(new MyMessage() { Content = "5" });

        var consumer = provider.GetRequiredService<IConsumerTestHarness<MyConsumer>>();

        Assert.IsTrue(await harness.Published.Any<MyMessage>());
        Assert.IsTrue(await consumer.Consumed.Any<MyMessage>());
    }

    [TestMethod]
    public async Task Should_Publish_And_Consume2()
    {
        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyConsumer>()
                    .Endpoint(e => e.Name = "my-queue");
            })
            .AddScoped<MyPublisher>()
            .BuildServiceProvider(true).CreateScope().ServiceProvider;

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var publisher = provider.GetRequiredService<MyPublisher>();

        await publisher.Publish("rferfwerfwerf");

        Assert.IsTrue(await harness.Sent.Any<MyMessage>());

        var sent = harness.Sent.Select<MyMessage>().FirstOrDefault();

        Assert.IsNotNull(sent);
        Assert.AreEqual("rferfwerfwerf", sent.Context.Message.Content);

        var consumerHarness = harness.GetConsumerHarness<MyConsumer>();

        Assert.IsTrue(await consumerHarness.Consumed.Any<MyMessage>());
    }
}