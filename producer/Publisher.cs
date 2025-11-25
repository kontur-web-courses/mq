using MassTransit;
using Models;

namespace producer;

public class Publisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public Publisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string content)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new User { Id = Guid.NewGuid(), Content = content });
    }
}