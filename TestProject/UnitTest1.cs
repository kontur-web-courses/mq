using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace TestProject;

[TestFixture]
public class MyConsumerTests
{
    [Test]
    public async Task MyConsumer_should_consume_Human_message()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyConsumer>();
                cfg.UsingInMemory((context, cfgInmem) =>
                {
                    cfgInmem.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        try
        {
            var human = new Human { Name = "Иван", IQ = 133 };
            await harness.Bus.Publish(human);

            Assert.IsTrue(await harness.Published.Any<Human>());
            Assert.IsTrue(await harness.Consumed.Any<Human>());

            var consumerHarness = harness.GetConsumerHarness<MyConsumer>();
            Assert.IsTrue(await consumerHarness.Consumed.Any<Human>(
                x => x.Context.Message.Name == "Иван" && x.Context.Message.IQ == 133));
        }
        finally
        {
            await harness.Stop();
        }
    }
}