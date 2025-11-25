using FluentAssertions;
using MassTransit;
using MassTransit.Internals;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace consumer.Tests;

[TestFixture]
public class OrderExecutorTests
{
	[Test]
	public async Task Consume_ShouldProcessOrderMessage_WhenMessageIsSent()
	{
		// Arrange
		await using var provider = new ServiceCollection()
			.AddMassTransitTestHarness(cfg => { cfg.AddConsumer<OrderExecutor>(); })
			.BuildServiceProvider(true);

		var harness = provider.GetRequiredService<ITestHarness>();
		await harness.Start();

		var orderId = Guid.NewGuid();
		var orderMessage = new OrderMessage
		{
			OrderId = orderId,
			Amount = 150.75m,
			CustomerName = "Test Customer"
		};

		// Act
		await harness.Bus.Publish(orderMessage);

		// Assert
		var consumed = await harness.Consumed.Any<OrderMessage>();
		consumed.Should().BeTrue();

		var consumerHarness = harness.GetConsumerHarness<OrderExecutor>();
		var consumerConsumed = await consumerHarness.Consumed.Any<OrderMessage>();
		consumerConsumed.Should().BeTrue();

		var consumedMessage = harness.Consumed.Select<OrderMessage>().FirstOrDefault()?.Context.Message;
		consumedMessage.Should().NotBeNull();
		consumedMessage.OrderId.Should().Be(orderId);
		consumedMessage.Amount.Should().Be(150.75m);
		consumedMessage.CustomerName.Should().Be("Test Customer");
	}

	[Test]
	public async Task Consume_ShouldProcessMultipleOrderMessages_WhenMultipleMessagesAreSent()
	{
		// Arrange
		await using var provider = new ServiceCollection()
			.AddMassTransitTestHarness(cfg => { cfg.AddConsumer<OrderExecutor>(); })
			.BuildServiceProvider(true);

		var harness = provider.GetRequiredService<ITestHarness>();
		await harness.Start();

		var messages = new[]
		{
			new OrderMessage { OrderId = Guid.NewGuid(), Amount = 100.0m, CustomerName = "Customer 1" },
			new OrderMessage { OrderId = Guid.NewGuid(), Amount = 200.0m, CustomerName = "Customer 2" },
			new OrderMessage { OrderId = Guid.NewGuid(), Amount = 300.0m, CustomerName = "Customer 3" }
		};

		// Act
		foreach (var message in messages)
		{
			await harness.Bus.Publish(message);
		}

		// Assert
		var consumedCount = harness.Consumed.Select<OrderMessage>().Count();
		consumedCount.Should().Be(3);

		var consumerHarness = harness.GetConsumerHarness<OrderExecutor>();
		var consumerConsumedCount = (await consumerHarness.Consumed.SelectAsync<OrderMessage>().ToListAsync()).Count;
		consumerConsumedCount.Should().Be(3);
	}
}