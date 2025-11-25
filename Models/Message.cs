namespace Models;

public class Message(string content)
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = content;
}