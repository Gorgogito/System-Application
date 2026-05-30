namespace BDAplication.Domain.Entities;

public class SubTask
{
    public int Id { get; set; }
    public int BoardTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public BoardTask BoardTask { get; set; } = default!;
}
