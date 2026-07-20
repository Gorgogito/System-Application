using BDAplication.Application.DTOs.Attachments;

namespace BDAplication.Application.Interfaces;

public interface IAttachmentService
{
    Task<IEnumerable<AttachmentDto>> GetAllAsync(string? search = null);
    Task<IEnumerable<AttachmentDto>> GetByEntityAsync(string entityType, int entityId);
    Task<AttachmentDto> UploadAsync(string entityType, int entityId, Stream fileStream,
        string fileName, string contentType, long fileSize, string description, string comment,
        DateTime documentDate, int? documentConceptId, decimal? amount, string user);
    Task<AttachmentDto> UpdateAsync(UpdateAttachmentRequest request, string user);
    Task DeleteAsync(int id);
    Task<string> GetSasUrlAsync(int id);
}
