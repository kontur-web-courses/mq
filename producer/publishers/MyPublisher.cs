using MassTransit;
using Models;

namespace producer.publishers;

public class MyPublisher(ISendEndpointProvider sendEndpointProvider)
{
    public async Task Publish(string content)
    {
        var publishEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new MyMessage { MessageId = Guid.NewGuid(), Content = content });
    }
}