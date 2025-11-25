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
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            { })
            .AddScoped<MyPublisher>()
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        using var scope = provider.CreateScope();
        var myPublisher = scope.ServiceProvider.GetRequiredService<MyPublisher>();
        
        var testText = "Hello from DI Test!";
        
        await myPublisher.Publish(testText);
        
        Assert.True(await harness.Sent.Any<Message>(), "Сообщение не попало в отправленные");

        var sentMessages = harness.Sent.Select<Message>().ToList();
        var messageContext = sentMessages.First();

        Assert.Equal(testText, messageContext.Context.Message.Content);

        Assert.Contains("my-queue", messageContext.Context.DestinationAddress?.AbsoluteUri);
    }
}