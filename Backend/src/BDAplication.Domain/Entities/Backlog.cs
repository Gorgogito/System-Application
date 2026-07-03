using BDAplication.Domain.Enums;

namespace BDAplication.Domain.Entities;

public class Backlog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public bool IsActive { get; set; } = true;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public ICollection<Planner> Planners { get; set; } = new List<Planner>();
}
