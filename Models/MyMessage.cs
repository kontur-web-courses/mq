namespace Models;

public class MyMessage(string content)
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = content;
}