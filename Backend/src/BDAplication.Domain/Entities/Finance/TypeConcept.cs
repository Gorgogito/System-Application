namespace BDAplication.Domain.Entities.Finance;

public class TypeConcept
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;       // 0000000001
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}
