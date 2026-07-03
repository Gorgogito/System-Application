using BDAplication.Domain.Entities;

namespace BDAplication.Domain.Interfaces;

public interface IAttachmentRepository
{
    Task<IEnumerable<Attachment>> GetAllAsync(string? search = null);
    Task<IEnumerable<Attachment>> GetByEntityAsync(string entityType, int entityId);
    Task<Attachment?> GetByIdAsync(int id);
    Task<Attachment> CreateAsync(Attachment attachment);
    Task<Attachment> UpdateAsync(Attachment attachment);
    Task DeleteAsync(int id);
}
