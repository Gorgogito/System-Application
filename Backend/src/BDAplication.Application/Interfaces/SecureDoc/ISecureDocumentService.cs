using BDAplication.Application.DTOs.SecureDoc;

namespace BDAplication.Application.Interfaces.SecureDoc;

public interface ISecureDocumentService
{
    Task<IEnumerable<SecureDocumentListItemDto>> GetAllAsync(string requestingUser);
    Task<SecureDocumentDto?> GetByIdAsync(int id);
    Task<SecureDocumentDto> CreateAsync(CreateSecureDocumentRequest request, string user);
    Task<SecureDocumentDto?> AddVersionAsync(int documentId, AddVersionRequest request, string user);
    Task<DecryptedContentDto?> DecryptAsync(int documentId, DecryptRequest request, string requestingUser);
    Task<bool?> SoftDeleteAsync(int id, DecryptRequest request, string user);
}
