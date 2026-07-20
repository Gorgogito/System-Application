namespace BDAplication.Domain.Entities.SecureDoc;

public class SecureDocumentVersion
{
    public int Id { get; set; }
    public int SecureDocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string EncryptedContent { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public SecureDocument Document { get; set; } = null!;
}
