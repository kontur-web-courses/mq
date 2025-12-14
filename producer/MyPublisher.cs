using MassTransit;
using Models;

public class MyPublisher(ISendEndpointProvider sendEndpointProvider)
{
    public async Task Publish(string content)
    {
        var publishEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new Message { Id = Guid.NewGuid(), Text = content });
    }
}