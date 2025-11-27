using consumer;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace Tests;

public class MQTests
{
    [Test]
    public async Task MyConsumer_should_consume_MyMessage()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<MyConsumer>(); })
            .BuildServiceProvider(validateScopes: true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        try
        {
            await harness.Bus.Publish(new MyMessage
            {
                MessageId = Guid.NewGuid(),
                Content = "hello from test"
            });

            Assert.That(await harness.Consumed.Any<MyMessage>());

            var consumerHarness = harness.GetConsumerHarness<MyConsumer>();
            Assert.That(await consumerHarness.Consumed.Any<MyMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}