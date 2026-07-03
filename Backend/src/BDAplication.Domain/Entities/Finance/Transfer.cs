namespace BDAplication.Domain.Entities.Finance;

public class Transfer
{
    public int Id { get; set; }
    public int SourceAccountId { get; set; }
    public int SourceMovementId { get; set; }
    public int DestinyAccountId { get; set; }
    public int DestinyMovementId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public Account SourceAccount { get; set; } = default!;
    public Movement SourceMovement { get; set; } = default!;
    public Account DestinyAccount { get; set; } = default!;
    public Movement DestinyMovement { get; set; } = default!;
}
