using Models;
using MassTransit;

public class MyPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MyPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish()
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new MyMessage
        {
            Content = "Hello World!"
        });
    }
}
