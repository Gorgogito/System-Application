namespace BDAplication.Application.DTOs.Attachments;

public class UpdateAttachmentRequest
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public int? DocumentConceptId { get; set; }
}
