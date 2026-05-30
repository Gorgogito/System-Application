namespace BDAplication.Domain.Entities;

public class Planner
{
    public int Id { get; set; }
    public int BacklogId { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public Backlog Backlog { get; set; } = null!;
}
