using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;
using producer;
using Xunit;

namespace producer.Tests;

public class OrderPublisherTests
{
    [Fact]
    public async Task Publish_ShouldSendOrderMessageToQueue()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {

            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await using var scope = provider.CreateAsyncScope();
        var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        var publisher = new OrderPublisher(sendEndpointProvider);

        await publisher.Publish("John Doe", 1000.50m);

        Assert.True(await harness.Sent.Any<OrderMessage>());
        
        var sentMessage = harness.Sent.Select<OrderMessage>().FirstOrDefault();
        Assert.NotNull(sentMessage);
        Assert.Equal("John Doe", sentMessage.Context.Message.CustomerName);
        Assert.Equal(1000.50m, sentMessage.Context.Message.Amount);
        Assert.NotEqual(Guid.Empty, sentMessage.Context.Message.OrderId);
        Assert.True(sentMessage.Context.Message.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Publish_ShouldGenerateUniqueOrderIds()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {

            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await using var scope = provider.CreateAsyncScope();
        var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        var publisher = new OrderPublisher(sendEndpointProvider);

        await publisher.Publish("Customer 1", 100);
        await publisher.Publish("Customer 2", 200);

        Assert.True(await harness.Sent.Any<OrderMessage>());
        
        var sentMessages = harness.Sent.Select<OrderMessage>().ToList();
        Assert.Equal(2, sentMessages.Count);
        
        var orderIds = sentMessages.Select(m => m.Context.Message.OrderId).ToList();
        Assert.Equal(2, orderIds.Distinct().Count());
    }

    [Theory]
    [InlineData("Alice", 500.00)]
    [InlineData("Bob", 1500.75)]
    [InlineData("Charlie", 250.50)]
    public async Task Publish_ShouldSendMessagesWithCorrectData(string customerName, decimal amount)
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {

            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        await using var scope = provider.CreateAsyncScope();
        var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        var publisher = new OrderPublisher(sendEndpointProvider);

        await publisher.Publish(customerName, amount);

        Assert.True(await harness.Sent.Any<OrderMessage>());
        
        var sentMessage = harness.Sent.Select<OrderMessage>().FirstOrDefault();
        Assert.NotNull(sentMessage);
        Assert.Equal(customerName, sentMessage.Context.Message.CustomerName);
        Assert.Equal(amount, sentMessage.Context.Message.Amount);
    }
}
