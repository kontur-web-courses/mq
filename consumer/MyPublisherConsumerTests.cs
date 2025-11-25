using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Models;
using NUnit.Framework;
using producer;
using consumer;

namespace tests
{
    [TestFixture]
    public class MyPublisherConsumerTests
    {
        private InMemoryTestHarness harness = null!;
        private MyPublisher publisher = null!;

        [SetUp]
        public async Task SetUp()
        {
            harness = new InMemoryTestHarness();
            harness.Consumer<MyConsumer>();

            await harness.Start();

            publisher = new MyPublisher(harness.Bus);
        }

        [TearDown]
        public async Task TearDown()
        {
            await harness.Stop();
            harness.Dispose();
        }

        [Test]
        public async Task TestReceiveMessage()
        {
            const string content = "Test test";
            await publisher.Publish(content);
            ConsumeContext<MyMessage>? consumed = null;

            foreach (var ctx in harness.Consumed.Select<MyMessage>())
            {
                consumed = ctx.Context;
                break;
            }

            Assert.That(consumed, Is.Not.Null, "Сообщение не было получено");
            Assert.That(consumed!.Message.Content, Is.EqualTo(content));
            Assert.That(consumed.Message.MessageId, Is.Not.EqualTo(Guid.Empty));
        }
    }
}