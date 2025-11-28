using MassTransit;

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
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:msg-queue"));
        await publishEndpoint.Send(new Message
        {
            MessageId = Guid.NewGuid(), 
            Content = content
        });
    }
}