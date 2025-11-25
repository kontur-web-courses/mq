namespace Models;

public class OrderMessage
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
