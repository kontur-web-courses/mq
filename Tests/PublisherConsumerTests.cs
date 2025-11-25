using consumer;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;
using producer.publishers;

namespace Tests;

public class PublisherConsumerTests
{
    [Fact]
    public async Task Publisher_Should_Send_Message_And_Consumer_Should_Handle_It()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<MyConsumer>();
                x.UsingInMemory((context, cfg) => 
                    cfg.ReceiveEndpoint("my-queue", e => 
                        e.ConfigureConsumer<MyConsumer>(context)));
            })
            .AddScoped<MyPublisher>()
            .BuildServiceProvider();

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var publisher = provider.GetRequiredService<MyPublisher>();
        const string content = "some content";
        await publisher.Publish(content);

        Assert.True(await harness.Sent.Any<MyMessage>());
        Assert.True(await harness.Consumed.Any<MyMessage>());
        
        var consumerHarness = harness.GetConsumerHarness<MyConsumer>();
        var consumed = await consumerHarness.Consumed.SelectAsync<MyMessage>().FirstOrDefault();
        Assert.NotNull(consumed);
        Assert.Equal(content, consumed.Context.Message.Content);
    }
}