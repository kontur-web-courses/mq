using MassTransit;

namespace Models;

public class MyMessage
{
    public Guid MessageId { get; set; } = Guid.NewGuid();

    public string Content { get; set; } = String.Empty;
}