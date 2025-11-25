using MassTransit;
using Models;

namespace consumer;

public sealed class OrderExecutor : IConsumer<OrderMessage>
{
	public async Task Consume(ConsumeContext<OrderMessage> context)
	{
		var message = context.Message;
		Console.WriteLine($"Executing order {message.OrderId} with amount {message.Amount} for customer {message.CustomerName}");
		await Task.Delay(TimeSpan.FromSeconds(5));
		Console.WriteLine($"Executed order {message.OrderId}");
	}
}