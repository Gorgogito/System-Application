using BDAplication.Domain.Enums;

namespace BDAplication.Application.Services.TaskBoard;

public static class TaskDueHelper
{
    public static DueStatus Compute(
        DateTime? startDate, DateTime? dueDate, int? alertHours, TaskBoardStatus status)
    {
        if (status is TaskBoardStatus.Completed or TaskBoardStatus.Cancelled or TaskBoardStatus.Suspended)
            return DueStatus.Done;

        if (!dueDate.HasValue)
            return DueStatus.NotScheduled;

        var now = DateTime.UtcNow;

        if (startDate.HasValue && now < startDate.Value)
            return DueStatus.Scheduled;

        if (now > dueDate.Value)
            return DueStatus.Overdue;

        if (alertHours.HasValue && (dueDate.Value - now).TotalHours <= alertHours.Value)
            return DueStatus.DueSoon;

        return DueStatus.OnTime;
    }

    public static string Label(DueStatus status) => status switch
    {
        DueStatus.NotScheduled => "Sin programar",
        DueStatus.Scheduled    => "Programada",
        DueStatus.OnTime       => "En tiempo",
        DueStatus.DueSoon      => "Por vencer",
        DueStatus.Overdue      => "Vencida",
        DueStatus.Done         => "Finalizada",
        _                      => status.ToString()
    };

    // Ordinal para sorting: Overdue(0) > DueSoon(1) > OnTime/Scheduled(2) > NotScheduled/Done(3)
    public static int SortOrder(DueStatus s) => s switch
    {
        DueStatus.Overdue      => 0,
        DueStatus.DueSoon      => 1,
        DueStatus.OnTime       => 2,
        DueStatus.Scheduled    => 3,
        _                      => 4
    };
}
