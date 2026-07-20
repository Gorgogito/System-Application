namespace BDAplication.Web.Models;

public class AttachmentModel
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public int? DocumentConceptId { get; set; }
    public string? DocumentConceptName { get; set; }
    public decimal? Amount { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }

    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    public bool IsPdf => ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);
    public bool HasAmount => Amount.HasValue && Amount.Value > 0;
    public string AmountDisplay => Amount.HasValue ? $"S/ {Amount.Value:N2}" : string.Empty;

    public string FileSizeDisplay => FileSizeBytes switch
    {
        < 1024 => $"{FileSizeBytes} B",
        < 1_048_576 => $"{FileSizeBytes / 1024.0:N1} KB",
        _ => $"{FileSizeBytes / 1_048_576.0:N1} MB"
    };
}

public class UpdateAttachmentRequest
{
    public string Description { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public int? DocumentConceptId { get; set; }
    public decimal? Amount { get; set; }
}
