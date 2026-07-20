using BDAplication.Domain.Entities.SecureDoc;

namespace BDAplication.Domain.Interfaces.SecureDoc;

public interface ISecureDocumentRepository
{
    Task<IEnumerable<SecureDocument>> GetAllAsync(string? createdBy = null);
    Task<SecureDocument?> GetByIdWithVersionsAsync(int id);
    Task<SecureDocumentVersion?> GetVersionAsync(int documentId, int versionId);
    Task<SecureDocument> CreateAsync(SecureDocument document);
    Task<SecureDocumentVersion> AddVersionAsync(SecureDocumentVersion version);
    Task<SecureDocument?> UpdateHeaderAsync(int id, string title, string modifiedBy);
    Task<bool> SoftDeleteAsync(int id, string modifiedBy);
}
