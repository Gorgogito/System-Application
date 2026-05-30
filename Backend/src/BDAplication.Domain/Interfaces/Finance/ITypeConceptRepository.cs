using BDAplication.Domain.Entities.Finance;

namespace BDAplication.Domain.Interfaces.Finance;

public interface ITypeConceptRepository
{
    Task<IEnumerable<TypeConcept>> GetAllAsync();
    Task<TypeConcept?> GetByCodeAsync(string code);
    Task<TypeConcept?> GetByIdAsync(int id);
    Task<TypeConcept> CreateAsync(TypeConcept typeConcept);
    Task<TypeConcept> UpdateAsync(TypeConcept typeConcept);
    Task DeleteAsync(int id);
    Task<string> GenerateCodeAsync();
    Task<bool> IsUsedInMovementsAsync(int id);
}
