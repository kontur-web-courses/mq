namespace Models;

public class Model
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}