using consumer;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;
using NUnit.Framework;
using producer;

namespace Producer.Tests
{
    [TestFixture]
    public class MassTransitTests
    {
        private ServiceProvider? _provider;
        private ITestHarness? _harness;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            var services = new ServiceCollection();

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyConsumer>();
                cfg.UsingInMemory((context, cfgg) =>
                {
                    cfgg.ConfigureEndpoints(context);
                });
            });
            services.AddScoped<MyPublisher>();
            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyConsumer>();

                cfg.UsingInMemory((context, cfgg) =>
                {
                    cfgg.ReceiveEndpoint("my-queue", e =>
                    {
                        e.ConfigureConsumer<MyConsumer>(context);
                    });
                });
            });
            _provider = services.BuildServiceProvider();
            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown() => await _harness.Stop();

        [Test]
        public async Task Should_publish_message_via_MyPublisher_and_consumer_receives_it()
        {
            var publisher = _harness.Provider.GetRequiredService<MyPublisher>();
            await publisher.Publish("FirstTest", DateOnly.MaxValue,  "Прив кд чд?");
            Assert.That(await _harness.Consumed.Any<Model>(), Is.True.After(5000, 100),
                "Консьюмер должен получить сообщение");

            var consumed = _harness.Consumed.Select<Model>().FirstOrDefault();
            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed!.Context.Message.Message, Is.EqualTo("Прив кд чд?"));
        }

        [Test]
        public async Task Should_send_directly_to_queue_my_queue_and_consumer_receives_it()
        {
            var endpoint = await _harness.Bus.GetSendEndpoint(new Uri("queue:my-queue"));
            var id = Guid.NewGuid();
            var testMessage = new Model
            {
                Id = id,
                Message = "Прямой Send",
                Date = DateTime.Now,
                Name = $"Taska {id}",
            };

            await endpoint.Send(testMessage);
            Assert.That(await _harness.Sent.Any<Model>(), Is.True);
            Assert.That(await _harness.Consumed.Any<Model>(), Is.True);

            var consumed = _harness.Consumed.Select<Model>()
                .FirstOrDefault(c => c.Context.Message.Id == testMessage.Id);

            Assert.That(consumed, Is.Not.Null);
            Assert.That(consumed!.Context.Message.Message, Is.EqualTo("Прямой Send в NUnit"));
        }

        [Test]
        public async Task Consumer_should_be_registered_and_ready()
        {
            var consumerTestHarness = _harness.GetConsumerHarness<MyConsumer>();

            Assert.That(await consumerTestHarness.Consumed.Any<IModel>(), Is.False.After(5000, 100)); 
            
            var id = Guid.NewGuid();

            await _harness.Bus.Publish(new Model
            {
                Id = id,
                Message = "Тест готовности",
                Date = DateTime.Now,
                Name = $"Ready Taska {id}",
            });

            Assert.That(await consumerTestHarness.Consumed.Any<Model>(), Is.True.After(5000, 100),
                "Наш конкретный консьюмер должен получить сообщение");
        }
    }
}