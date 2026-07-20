namespace BDAplication.Web.Models;

public class SecureDocumentListItemModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int VersionCount { get; set; }
    public DateTime LastModified { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class SecureDocumentVersionModel
{
    public int Id { get; set; }
    public int VersionNumber { get; set; }
    public string EncryptedContent { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class SecureDocumentModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<SecureDocumentVersionModel> Versions { get; set; } = new();
}

public class DecryptedContentModel
{
    public int DocumentId { get; set; }
    public int VersionId { get; set; }
    public int VersionNumber { get; set; }
    public string HtmlContent { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public string DecryptedBy { get; set; } = string.Empty;
}

public class CreateSecureDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; } = DateTime.Now;
}

public class AddVersionRequest
{
    public string HtmlContent { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; } = DateTime.Now;
}

public class DecryptRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? VersionId { get; set; }
}
