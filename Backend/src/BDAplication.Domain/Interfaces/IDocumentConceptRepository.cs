using BDAplication.Domain.Entities;

namespace BDAplication.Domain.Interfaces;

public interface IDocumentConceptRepository
{
    Task<IEnumerable<DocumentConcept>> GetAllActiveAsync();
    Task<DocumentConcept?> GetByIdAsync(int id);
    Task<DocumentConcept> CreateAsync(DocumentConcept concept);
    Task<DocumentConcept> UpdateAsync(DocumentConcept concept);
    Task DeleteAsync(int id);
    Task<string> GenerateCodeAsync();
    Task<bool> IsUsedAsync(int id);
}
