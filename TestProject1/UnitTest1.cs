using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using consumer;
using Models;

namespace ProducerConsumer.Tests;

public class RabbitMqTests
{
    private ServiceProvider _provider = null!;
    private ITestHarness _harness = null!;

    private IServiceScope _scope = null!;

    [SetUp]
    public async Task Setup()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<MyConsumer>();
            })
            .AddScoped<MyPublisher>()
            .BuildServiceProvider(true);

        _harness = _provider.GetRequiredService<ITestHarness>();
        await _harness.Start();

        // Создаем scoped-сервисы для теста
        _scope = _provider.CreateScope();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _harness.Stop();
        await _provider.DisposeAsync();
        _scope.Dispose();
    }

    [Test]
    public async Task Publisher_Should_Send_And_Consumer_Should_Receive()
    {
        // получаем scoped MyPublisher
        var publisher = _scope.ServiceProvider.GetRequiredService<MyPublisher>();
        var text = "hello test";

        await publisher.Publish(text);

        // сообщение отправлено
        Assert.That(await _harness.Sent.Any<Class1>(), Is.True);

        // сообщение получено consumer
        Assert.That(await _harness.Consumed.Any<Class1>(), Is.True);

        // дополнительно проверяем payload
        var consumed = _harness.Consumed
            .Select<Class1>()
            .First().Context.Message;

        Assert.That(consumed.message, Is.EqualTo(text));
    }
}