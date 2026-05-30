using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;
using BDAplication.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BDAplication.Persistence.Repositories.Finance;

public class TypeConceptRepository : ITypeConceptRepository
{
    private readonly ApplicationDbContext _db;
    public TypeConceptRepository(ApplicationDbContext db) => _db = db;

    public async Task<IEnumerable<TypeConcept>> GetAllAsync() =>
        await _db.TypeConcepts.OrderBy(tc => tc.Code).ToListAsync();

    public async Task<TypeConcept?> GetByCodeAsync(string code) =>
        await _db.TypeConcepts.FirstOrDefaultAsync(tc => tc.Code == code);

    public async Task<TypeConcept?> GetByIdAsync(int id) =>
        await _db.TypeConcepts.FindAsync(id);

    public async Task<TypeConcept> CreateAsync(TypeConcept typeConcept)
    {
        _db.TypeConcepts.Add(typeConcept);
        await _db.SaveChangesAsync();
        return typeConcept;
    }

    public async Task<TypeConcept> UpdateAsync(TypeConcept typeConcept)
    {
        _db.TypeConcepts.Update(typeConcept);
        await _db.SaveChangesAsync();
        return typeConcept;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.TypeConcepts.FindAsync(id) ?? throw new KeyNotFoundException();
        _db.TypeConcepts.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<string> GenerateCodeAsync()
    {
        var last = await _db.TypeConcepts
            .OrderByDescending(tc => tc.Code)
            .Select(tc => (string?)tc.Code)
            .FirstOrDefaultAsync();
        if (last == null) return "0000000001";
        var num = int.Parse(last) + 1;
        return num.ToString("D10");
    }

    public async Task<bool> IsUsedInMovementsAsync(int id) =>
        await _db.Movements.AnyAsync(m => m.TypeConceptId == id);
}
