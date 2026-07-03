namespace BDAplication.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;     // "Movement", "Transfer", "BoardTask", etc.
    public int EntityId { get; set; }
    public string OriginalName { get; set; } = string.Empty;   // Nombre original del archivo
    public string StoredName { get; set; } = string.Empty;     // GUID.ext — nombre en Blob Storage
    public string BlobPath { get; set; } = string.Empty;       // {EntityType}/{EntityId}/{StoredName}
    public string ContentType { get; set; } = string.Empty;    // MIME type
    public string Extension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public int? DocumentConceptId { get; set; }
    public DocumentConcept? DocumentConcept { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }
}
