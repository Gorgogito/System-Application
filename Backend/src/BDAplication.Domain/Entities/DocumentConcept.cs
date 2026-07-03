namespace BDAplication.Domain.Entities;

public class DocumentConcept
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;        // DC00000001
    public string Name { get; set; } = string.Empty;        // "Pago hipoteca", "Pensión familiar", etc.
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
