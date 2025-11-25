using MassTransit;
using Models;

namespace consumer;

public sealed class OrderExecutor : IConsumer<OrderMessage>
{
	public Task Consume(ConsumeContext<OrderMessage> context)
	{
		var message = context.Message;
		Console.WriteLine($"Executing order {message.OrderId} with amount {message.Amount} for customer {message.CustomerName}");
		Thread.Sleep(TimeSpan.FromMinutes(0.5));
		Console.WriteLine($"Finished executing order {message.OrderId}");
		return Task.CompletedTask;
	}
}