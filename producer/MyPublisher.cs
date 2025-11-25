using MassTransit;
using Models;

public class MyPublisher
{
    private readonly IPublishEndpoint _publish;

    public MyPublisher(IPublishEndpoint publish)
    {
        _publish = publish;
    }

    public Task Publish(string msg)
        => _publish.Publish(new Class1 { message = msg });
}