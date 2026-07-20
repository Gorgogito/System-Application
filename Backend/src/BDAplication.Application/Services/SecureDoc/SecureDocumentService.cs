using BDAplication.Application.DTOs.SecureDoc;
using BDAplication.Application.Interfaces;
using BDAplication.Application.Interfaces.SecureDoc;
using BDAplication.Domain.Entities.SecureDoc;
using BDAplication.Domain.Interfaces;
using BDAplication.Domain.Interfaces.SecureDoc;

namespace BDAplication.Application.Services.SecureDoc;

public class SecureDocumentService : ISecureDocumentService
{
    private readonly ISecureDocumentRepository _repo;
    private readonly IEncryptionService _encryption;
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;

    public SecureDocumentService(
        ISecureDocumentRepository repo,
        IEncryptionService encryption,
        IUserRepository userRepo,
        IPasswordHasher hasher)
    {
        _repo = repo;
        _encryption = encryption;
        _userRepo = userRepo;
        _hasher = hasher;
    }

    public async Task<IEnumerable<SecureDocumentListItemDto>> GetAllAsync(string requestingUser)
    {
        var docs = await _repo.GetAllAsync();
        return docs.Select(d => new SecureDocumentListItemDto(
            d.Id,
            d.Title,
            d.Versions.Count,
            d.ModifiedAt ?? d.CreatedAt,
            d.CreatedBy,
            d.IsActive));
    }

    public async Task<SecureDocumentDto?> GetByIdAsync(int id)
    {
        var doc = await _repo.GetByIdWithVersionsAsync(id);
        if (doc is null) return null;
        return MapToDto(doc);
    }

    public async Task<SecureDocumentDto> CreateAsync(CreateSecureDocumentRequest request, string user)
    {
        var encrypted = _encryption.Encrypt(request.HtmlContent);

        var doc = new Domain.Entities.SecureDoc.SecureDocument
        {
            Title = request.Title,
            CreatedBy = user,
            ModifiedBy = user,
        };

        var created = await _repo.CreateAsync(doc);

        var version = new SecureDocumentVersion
        {
            SecureDocumentId = created.Id,
            VersionNumber = 1,
            EncryptedContent = encrypted,
            Observation = request.Observation,
            DocumentDate = request.DocumentDate,
            CreatedBy = user,
        };

        await _repo.AddVersionAsync(version);

        var full = await _repo.GetByIdWithVersionsAsync(created.Id);
        return MapToDto(full!);
    }

    public async Task<SecureDocumentDto?> AddVersionAsync(int documentId, AddVersionRequest request, string user)
    {
        var doc = await _repo.GetByIdWithVersionsAsync(documentId);
        if (doc is null || !doc.IsActive) return null;

        var nextVersion = doc.Versions.Count + 1;
        var encrypted = _encryption.Encrypt(request.HtmlContent);

        var version = new SecureDocumentVersion
        {
            SecureDocumentId = documentId,
            VersionNumber = nextVersion,
            EncryptedContent = encrypted,
            Observation = request.Observation,
            DocumentDate = request.DocumentDate,
            CreatedBy = user,
        };

        await _repo.AddVersionAsync(version);
        await _repo.UpdateHeaderAsync(documentId, doc.Title, user);

        var full = await _repo.GetByIdWithVersionsAsync(documentId);
        return MapToDto(full!);
    }

    public async Task<DecryptedContentDto?> DecryptAsync(int documentId, DecryptRequest request, string requestingUser)
    {
        // Re-autenticación: valida credenciales contra la BD
        var user = await _userRepo.GetByUsernameAsync(request.Username);
        if (user is null || !user.IsActive || !_hasher.Verify(request.Password, user.PasswordHash))
            return null;

        var doc = await _repo.GetByIdWithVersionsAsync(documentId);
        if (doc is null || !doc.IsActive) return null;

        SecureDocumentVersion? version;
        if (request.VersionId.HasValue)
        {
            version = await _repo.GetVersionAsync(documentId, request.VersionId.Value);
        }
        else
        {
            version = doc.Versions
                .OrderByDescending(v => v.VersionNumber)
                .FirstOrDefault();
        }

        if (version is null) return null;

        var html = _encryption.Decrypt(version.EncryptedContent);

        return new DecryptedContentDto(
            documentId,
            version.Id,
            version.VersionNumber,
            html,
            version.Observation,
            version.DocumentDate,
            requestingUser);
    }

    public async Task<bool?> SoftDeleteAsync(int id, DecryptRequest request, string user)
    {
        var authUser = await _userRepo.GetByUsernameAsync(request.Username);
        if (authUser is null || !authUser.IsActive || !_hasher.Verify(request.Password, authUser.PasswordHash))
            return null;

        return await _repo.SoftDeleteAsync(id, user);
    }

    private static SecureDocumentDto MapToDto(Domain.Entities.SecureDoc.SecureDocument doc) =>
        new(
            doc.Id,
            doc.Title,
            doc.IsActive,
            doc.CreatedAt,
            doc.CreatedBy,
            doc.Versions
               .OrderByDescending(v => v.VersionNumber)
               .Select(v => new SecureDocumentVersionDto(
                   v.Id,
                   v.VersionNumber,
                   TruncateForDisplay(v.EncryptedContent),
                   v.Observation,
                   v.DocumentDate,
                   v.CreatedAt,
                   v.CreatedBy))
               .ToList());

    // Muestra solo los primeros 80 chars del cifrado — nunca el texto completo
    private static string TruncateForDisplay(string cipherText) =>
        cipherText.Length > 80 ? cipherText[..80] + "..." : cipherText;
}
