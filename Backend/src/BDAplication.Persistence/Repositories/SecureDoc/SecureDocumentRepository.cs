using BDAplication.Domain.Entities.SecureDoc;
using BDAplication.Domain.Interfaces.SecureDoc;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.SecureDoc;

public class SecureDocumentRepository : ISecureDocumentRepository
{
    private readonly ApplicationDbContext _db;

    public SecureDocumentRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<SecureDocument>> GetAllAsync(string? createdBy = null)
    {
        var query = _db.SecureDocuments
            .Include(d => d.Versions)
            .Where(d => d.IsActive);

        if (!string.IsNullOrEmpty(createdBy))
            query = query.Where(d => d.CreatedBy == createdBy);

        return await query
            .OrderByDescending(d => d.ModifiedAt ?? d.CreatedAt)
            .ToListAsync();
    }

    public async Task<SecureDocument?> GetByIdWithVersionsAsync(int id) =>
        await _db.SecureDocuments
            .Include(d => d.Versions.OrderByDescending(v => v.VersionNumber))
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<SecureDocumentVersion?> GetVersionAsync(int documentId, int versionId) =>
        await _db.SecureDocumentVersions
            .FirstOrDefaultAsync(v => v.Id == versionId && v.SecureDocumentId == documentId);

    public async Task<SecureDocument> CreateAsync(SecureDocument document)
    {
        _db.SecureDocuments.Add(document);
        await _db.SaveChangesAsync();
        return document;
    }

    public async Task<SecureDocumentVersion> AddVersionAsync(SecureDocumentVersion version)
    {
        _db.SecureDocumentVersions.Add(version);

        // Actualizar ModifiedAt del documento padre
        var doc = await _db.SecureDocuments.FindAsync(version.SecureDocumentId);
        if (doc is not null)
        {
            doc.ModifiedAt = DateTime.UtcNow;
            doc.ModifiedBy = version.CreatedBy;
        }

        await _db.SaveChangesAsync();
        return version;
    }

    public async Task<SecureDocument?> UpdateHeaderAsync(int id, string title, string modifiedBy)
    {
        var doc = await _db.SecureDocuments.FindAsync(id);
        if (doc is null) return null;

        doc.Title = title;
        doc.ModifiedBy = modifiedBy;
        doc.ModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return doc;
    }

    public async Task<bool> SoftDeleteAsync(int id, string modifiedBy)
    {
        var doc = await _db.SecureDocuments.FindAsync(id);
        if (doc is null) return false;

        doc.IsActive = false;
        doc.ModifiedBy = modifiedBy;
        doc.ModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }
}
