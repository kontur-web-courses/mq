using MassTransit;

public class MyPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MyPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
        Console.WriteLine("1");
    }

    public async Task Publish(string content)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:my-queue"));
        await publishEndpoint.Send(new Models.Model { Number = Guid.NewGuid(), Text = content });
    }
}