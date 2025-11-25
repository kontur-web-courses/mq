using MassTransit;
using Models;

namespace producer;

public class OrderPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string customerName, decimal amount)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new OrderMessage 
        { 
            OrderId = Guid.NewGuid(), 
            CustomerName = customerName,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        });
    }
}
