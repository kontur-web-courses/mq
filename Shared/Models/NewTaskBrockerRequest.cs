namespace Shared.Models;

public class NewTaskBrockerRequest
{
    public Guid MessageId { get; set; }

    public required string Content { get; set; }
}
