namespace Models;

public class MyMessage
{
    public Guid MessageId { get; set; }

    public required string Content { get; set; }

    public int RandomInt { get; set; } = new Random().Next();
}