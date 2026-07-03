using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories;

public class DocumentConceptRepository : IDocumentConceptRepository
{
    private readonly ApplicationDbContext _db;
    public DocumentConceptRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<DocumentConcept>> GetAllActiveAsync() =>
        await _db.DocumentConcepts
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<DocumentConcept?> GetByIdAsync(int id) =>
        await _db.DocumentConcepts.FindAsync(id);

    public async Task<DocumentConcept> CreateAsync(DocumentConcept concept)
    {
        _db.DocumentConcepts.Add(concept);
        await _db.SaveChangesAsync();
        return concept;
    }

    public async Task<DocumentConcept> UpdateAsync(DocumentConcept concept)
    {
        _db.DocumentConcepts.Update(concept);
        await _db.SaveChangesAsync();
        return concept;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.DocumentConcepts.FindAsync(id)
            ?? throw new KeyNotFoundException($"Concepto {id} no encontrado");
        entity.IsActive = false;
        await _db.SaveChangesAsync();
    }

    public async Task<string> GenerateCodeAsync()
    {
        var last = await _db.DocumentConcepts
            .OrderByDescending(c => c.Code)
            .Select(c => (string?)c.Code)
            .FirstOrDefaultAsync();
        if (last == null) return "DC00000001";
        var num = int.Parse(last[2..]) + 1;
        return $"DC{num:D8}";
    }

    public async Task<bool> IsUsedAsync(int id) =>
        await _db.Attachments.AnyAsync(a => a.DocumentConceptId == id);
}
