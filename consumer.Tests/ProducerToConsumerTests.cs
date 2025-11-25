using MassTransit.Internals;
using MassTransit.Testing;
using Models;
using NUnit.Framework;
using producer;

namespace consumer.Tests;

[TestFixture]
public class ProducerToConsumerTests
{
    private InMemoryTestHarness _harness;
    private MyPublisher _publisher;
    private ConsumerTestHarness<MyConsumer> _consumer;

    [SetUp]
    public async Task Setup()
    {
        _harness = new();
        _consumer = _harness.Consumer<MyConsumer>("my-queue");
        await _harness.Start();
        _publisher = new(_harness.Bus);
    }

    [TearDown]
    public async Task Teardown()
    {
        await _harness.Stop();
    }

    [Test]
    public async Task Producer_Should_Send_And_Consumer_Should_Receive()
    {
        await _publisher.Publish("test message");
        await Task.Delay(1000);
        var messages =
            await _consumer.Consumed.SelectAsync<VeryImportantTaskModel>().ToListAsync();
        Assert.That(messages.First().Context.Message.VeryImportantTaskData,
            Is.EqualTo("test message"));
    }
}
