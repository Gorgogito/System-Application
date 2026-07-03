namespace BDAplication.Application.DTOs.TaskBoard;

public record ResetMonthResult(
    int      MovedCount,
    int      SkippedCount,
    string   ExecutedBy,
    DateTime ExecutedAt);
