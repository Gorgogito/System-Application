namespace BDAplication.Web.Models;

public enum TaskBoardStatus
{
    Pending    = 1,
    InProgress = 2,
    Suspended  = 3,
    Cancelled  = 4,
    Completed  = 5
}

public enum DueStatus
{
    NotScheduled = 0,
    Scheduled    = 1,
    OnTime       = 2,
    DueSoon      = 3,
    Overdue      = 4,
    Done         = 5
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
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? AlertHoursBeforeDue { get; set; }
    public DueStatus DueStatus { get; set; }
    public string DueStatusLabel { get; set; } = string.Empty;
    public List<SubTaskModel> SubTasks { get; set; } = new();

    // Calcula DueStatus en tiempo real usando la hora actual del cliente
    public DueStatus CurrentDueStatus
    {
        get
        {
            if (Status is TaskBoardStatus.Completed or TaskBoardStatus.Cancelled or TaskBoardStatus.Suspended)
                return DueStatus.Done;
            if (DueDate is null) return DueStatus.NotScheduled;
            var now = DateTime.UtcNow;
            if (StartDate.HasValue && now < StartDate.Value) return DueStatus.Scheduled;
            if (now > DueDate.Value) return DueStatus.Overdue;
            if (AlertHoursBeforeDue.HasValue && (DueDate.Value - now).TotalHours <= AlertHoursBeforeDue.Value)
                return DueStatus.DueSoon;
            return DueStatus.OnTime;
        }
    }

    public string TimeRemainingText
    {
        get
        {
            if (DueDate is null) return string.Empty;
            var now = DateTime.UtcNow;
            var diff = DueDate.Value - now;

            if (StartDate.HasValue && now < StartDate.Value)
            {
                var toStart = StartDate.Value - now;
                return FormatSpan("📅 Inicia en", toStart);
            }

            if (diff.TotalSeconds < 0)
                return FormatSpan("🚨 Venció hace", -diff, past: true);

            return FormatSpan("⏰ Restan", diff);
        }
    }

    private static string FormatSpan(string prefix, TimeSpan span, bool past = false)
    {
        if (span.TotalMinutes < 60)
            return $"{prefix} {(int)span.TotalMinutes} min";
        if (span.TotalHours < 24)
            return $"{prefix} {(int)span.TotalHours} {(past ? "horas" : "horas")}";
        if (span.TotalDays < 2)
            return past ? $"{prefix} 1 día" : $"{prefix} {(int)span.TotalHours} horas";
        return $"{prefix} {(int)span.TotalDays} días";
    }
}

public class CreateTaskBoardRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? AlertHoursBeforeDue { get; set; }
}

public class UpdateTaskBoardRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? AlertHoursBeforeDue { get; set; }
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

public class ResetMonthResult
{
    public int      MovedCount   { get; set; }
    public int      SkippedCount { get; set; }
    public string   ExecutedBy   { get; set; } = string.Empty;
    public DateTime ExecutedAt   { get; set; }
}
