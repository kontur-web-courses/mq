using MassTransit;
using Models;

namespace producer;

public class MyPublisher(IPublishEndpoint  publishEndpoint)
{
    public async Task Publish(string content)
    {
        //var publishEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Publish(new MyMessage { MessageId = Guid.NewGuid(), Content = content });
    }
}