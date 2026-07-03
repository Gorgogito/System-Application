namespace BDAplication.Domain.Entities;

public class ResetMonthLog
{
    public int      Id           { get; set; }
    public string   ExecutedBy   { get; set; } = string.Empty;
    public DateTime ExecutedAt   { get; set; } = DateTime.UtcNow;
    public int      MovedCount   { get; set; }
    public int      SkippedCount { get; set; }
}
