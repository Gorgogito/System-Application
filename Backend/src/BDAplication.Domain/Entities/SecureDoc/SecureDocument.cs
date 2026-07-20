using BDAplication.Domain.Common;

namespace BDAplication.Domain.Entities.SecureDoc;

public class SecureDocument : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime? ModifiedAt { get; set; }

    public ICollection<SecureDocumentVersion> Versions { get; set; } = new List<SecureDocumentVersion>();
}
