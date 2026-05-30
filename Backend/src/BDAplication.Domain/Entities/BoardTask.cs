using BDAplication.Domain.Enums;

namespace BDAplication.Domain.Entities;

public class BoardTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskBoardStatus Status { get; set; } = TaskBoardStatus.Pending;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
}
