using MassTransit;
using Models;

public class MyPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MyPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string content)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await endpoint.Send(new MyMessage
        {
            MessageId = Guid.NewGuid(),
            Content = content
        });
    }
}