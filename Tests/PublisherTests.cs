using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection; // <-- Нужно для ServiceCollection
using Models;
using Xunit;

namespace Tests;

public class PublisherTests
{
    [Fact]
    public async Task SendMessage_Should_SendToSpecificQueue()
    {
        // 1. Настройка DI-контейнера (как в документации)
        await using var provider = new ServiceCollection()
            // Добавляем TestHarness
            .AddMassTransitTestHarness(cfg =>
            {
                // Если бы у нас были Consumer-ы, мы бы добавили их здесь:
                // cfg.AddConsumer<MyConsumer>();
            })
            // Регистрируем твой класс MyPublisher
            // (MassTransit сам подставит ему нужные зависимости)
            .AddScoped<MyPublisher>()
            .BuildServiceProvider(true);

        // 2. Получаем Harness и запускаем его
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        // 3. Достаем твой класс из контейнера (а не создаем через new)
        using var scope = provider.CreateScope();
        var myPublisher = scope.ServiceProvider.GetRequiredService<MyPublisher>();
        
        var testText = "Hello from DI Test!";

        // --- Act (Действие) ---
        await myPublisher.Publish(testText);

        // --- Assert (Проверка) ---
        
        // Проверяем, что сообщение MyMessage действительно было ОТПРАВЛЕНО (Sent)
        // Используем Sent, так как в коде MyPublisher используется метод .Send()
        Assert.True(await harness.Sent.Any<MyMessage>(), "Сообщение не попало в отправленные");

        // Проверяем детали сообщения
        var sentMessages = harness.Sent.Select<MyMessage>().ToList();
        var messageContext = sentMessages.First();

        Assert.Equal(testText, messageContext.Context.Message.Content);
        
        // Проверяем, что адрес назначения содержит имя очереди "my-queue"
        Assert.Contains("my-queue", messageContext.Context.DestinationAddress?.AbsoluteUri);
    }
}