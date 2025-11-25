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

    public async Task Publish(string name, DateOnly date, string content)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        var datetime = date.ToDateTime(TimeOnly.MinValue);
        await publishEndpoint
            .Send(new Model { Id = Guid.NewGuid(), Name = name, Date = datetime, Message = content });
    }
}