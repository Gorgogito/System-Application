using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly ApplicationDbContext _db;
    public AttachmentRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<Attachment>> GetAllAsync(string? search = null)
    {
        var q = _db.Attachments.Include(a => a.DocumentConcept).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(a => a.OriginalName.Contains(search) ||
                             a.Description.Contains(search) ||
                             a.Comment.Contains(search) ||
                             a.UserCreated.Contains(search));
        return await q.OrderByDescending(a => a.DateCreated).ToListAsync();
    }

    public async Task<IEnumerable<Attachment>> GetByEntityAsync(string entityType, int entityId) =>
        await _db.Attachments
            .Include(a => a.DocumentConcept)
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.DateCreated)
            .ToListAsync();

    public async Task<Attachment?> GetByIdAsync(int id) =>
        await _db.Attachments.Include(a => a.DocumentConcept).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Attachment> CreateAsync(Attachment attachment)
    {
        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync();
        return attachment;
    }

    public async Task<Attachment> UpdateAsync(Attachment attachment)
    {
        _db.Attachments.Update(attachment);
        await _db.SaveChangesAsync();
        return attachment;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Attachments.FindAsync(id)
            ?? throw new KeyNotFoundException($"Adjunto {id} no encontrado");
        _db.Attachments.Remove(entity);
        await _db.SaveChangesAsync();
    }
}
