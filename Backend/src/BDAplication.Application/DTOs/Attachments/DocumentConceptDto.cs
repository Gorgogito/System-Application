namespace BDAplication.Application.DTOs.Attachments;

public record DocumentConceptDto(
    int Id,
    string Code,
    string Name,
    string Description,
    bool IsActive,
    string CreatedBy,
    DateTime CreatedDate
);

public class CreateDocumentConceptRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateDocumentConceptRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
