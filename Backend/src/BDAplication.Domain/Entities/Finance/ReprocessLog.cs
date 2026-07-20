namespace BDAplication.Domain.Entities.Finance;

public class ReprocessLog
{
    public int Id { get; set; }
    public string ExecutedBy { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public bool IsSimulation { get; set; }
    public int AccountsProcessed { get; set; }
    public int MovementsRecalculated { get; set; }
    public int InconsistenciesFound { get; set; }
    public string AccountIdsJson { get; set; } = "[]";
    public string LogDetailsJson { get; set; } = "[]";
}
