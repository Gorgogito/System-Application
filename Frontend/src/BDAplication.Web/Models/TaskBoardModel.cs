namespace BDAplication.Web.Models;

public enum TaskBoardStatus
{
    Pending = 1,
    InProgress = 2,
    Suspended = 3,
    Cancelled = 4,
    Completed = 5
}

public class SubTaskModel
{
    public int Id { get; set; }
    public int BoardTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime DateCreated { get; set; }
}

public class TaskBoardModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public TaskBoardStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool IsActive { get; set; }
    public List<SubTaskModel> SubTasks { get; set; } = new();
}

public class CreateTaskBoardRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class UpdateTaskBoardRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
}

public class MoveTaskBoardRequest
{
    public int TaskId { get; set; }
    public TaskBoardStatus Status { get; set; }
}

public class CreateSubTaskRequest
{
    public int BoardTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class ToggleSubTaskRequest
{
    public int SubTaskId { get; set; }
}
