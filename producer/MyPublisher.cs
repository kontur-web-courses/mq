using MassTransit;
using Models;

namespace producer;

public class MyPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MyPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(string content)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new VeryImportantTaskModel { Id = Guid.NewGuid(), VeryImportantTaskData = content });
    }
}