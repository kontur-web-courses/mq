using MassTransit;
using Models;

namespace producer;

public class MyPublisher
{
    private readonly ISendEndpointProvider sendEndpointProvider;

    public MyPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        this.sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string customerName, decimal amount)
    {
        var publishEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new PublishMessage
        {
            MessageId = Guid.NewGuid(),
            CustomerName = customerName,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        });
    }
}