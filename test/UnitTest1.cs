using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;

public class MassTransitTests
{
    [Fact]
    public async Task Should_Publish_Message_To_Queue()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<MyMessage>(async (ConsumeContext<MyMessage> context) =>
                {
                    await Task.CompletedTask;
                });

                cfg.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider();

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var bus = provider.GetRequiredService<IBus>();

            await bus.Publish(new MyMessage
            {
                MessageId = Guid.NewGuid(),
                Content = "Test message"
            });

            Assert.True(await harness.Published.Any<MyMessage>());
            Assert.True(await harness.Consumed.Any<MyMessage>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}