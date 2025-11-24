using MassTransit;
using Shared.Models;

namespace producer.Brokers.RabbitMq;

public class MyRabbitMqPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MyRabbitMqPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string content)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new NewTaskBrockerRequest { MessageId = Guid.NewGuid(), Content = content });
    }
}