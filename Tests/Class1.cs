using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Xunit;

namespace Tests;

public class Tests
{
    [Fact]
    public async Task Should_Publish_Message_To_Queue()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<User>(async (ConsumeContext<User> context) =>
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

            await bus.Publish(new User
            {
                Id = Guid.NewGuid(),
                Content = "Test message"
            });

            Assert.True(await harness.Published.Any<User>());
            Assert.True(await harness.Consumed.Any<User>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}