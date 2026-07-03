using BDAplication.Application.DTOs.Attachments;
using BDAplication.Application.Interfaces;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services;

public class AttachmentService : IAttachmentService
{
    private static readonly HashSet<string> _allowedMime = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/gif", "image/webp", "application/pdf"
    };

    private readonly IAttachmentRepository _repo;
    private readonly IBlobStorageService _blob;

    public AttachmentService(IAttachmentRepository repo, IBlobStorageService blob)
    {
        _repo = repo;
        _blob = blob;
    }

    public async Task<IEnumerable<AttachmentDto>> GetAllAsync(string? search = null)
    {
        var list = await _repo.GetAllAsync(search);
        return list.Select(ToDto);
    }

    public async Task<IEnumerable<AttachmentDto>> GetByEntityAsync(string entityType, int entityId)
    {
        var list = await _repo.GetByEntityAsync(entityType, entityId);
        return list.Select(ToDto);
    }

    public async Task<AttachmentDto> UploadAsync(string entityType, int entityId,
        Stream fileStream, string fileName, string contentType, long fileSize,
        string description, string comment, DateTime documentDate, int? documentConceptId, string user)
    {
        if (!_allowedMime.Contains(contentType))
            throw new ArgumentException($"Tipo de archivo no permitido: {contentType}");

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var storedName = $"{Guid.NewGuid()}{ext}";
        var blobPath = $"{entityType.ToLower()}/{entityId}/{storedName}";

        await _blob.UploadAsync(fileStream, blobPath, contentType);

        Attachment attachment;
        try
        {
            attachment = await _repo.CreateAsync(new Attachment
            {
                EntityType = entityType,
                EntityId = entityId,
                OriginalName = Path.GetFileName(fileName),
                StoredName = storedName,
                BlobPath = blobPath,
                ContentType = contentType,
                Extension = ext,
                FileSizeBytes = fileSize,
                Description = description,
                Comment = comment,
                DocumentDate = documentDate,
                DocumentConceptId = documentConceptId,
                UserCreated = user,
                DateCreated = DateTime.UtcNow
            });
        }
        catch
        {
            await _blob.DeleteAsync(blobPath);
            throw;
        }

        return ToDto(attachment);
    }

    public async Task<AttachmentDto> UpdateAsync(UpdateAttachmentRequest request, string user)
    {
        var attachment = await _repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Adjunto {request.Id} no encontrado");

        attachment.Description = request.Description;
        attachment.Comment = request.Comment;
        attachment.DocumentDate = request.DocumentDate;
        attachment.DocumentConceptId = request.DocumentConceptId;
        attachment.UserModified = user;
        attachment.DateModified = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(attachment);
        return ToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var attachment = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Adjunto {id} no encontrado");

        await _repo.DeleteAsync(id);
        await _blob.DeleteAsync(attachment.BlobPath);
    }

    public async Task<string> GetSasUrlAsync(int id)
    {
        var attachment = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Adjunto {id} no encontrado");

        return await _blob.GetSasUrlAsync(attachment.BlobPath, TimeSpan.FromHours(1));
    }

    private static AttachmentDto ToDto(Attachment a) =>
        new(a.Id, a.EntityType, a.EntityId, a.OriginalName, a.BlobPath,
            a.ContentType, a.Extension, a.FileSizeBytes,
            a.Description, a.Comment, a.DocumentDate,
            a.DocumentConceptId, a.DocumentConcept?.Name,
            a.UserCreated, a.DateCreated, a.UserModified, a.DateModified);
}
