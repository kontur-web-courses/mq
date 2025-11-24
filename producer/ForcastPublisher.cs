using MassTransit;
using Models;

namespace producer;

public class ForcastPublisher
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ForcastPublisher(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish(DailyWeatherForecast forecast)
    {
        var publishEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:forecast-queue"));
        await publishEndpoint.Send(forecast);
    }
}