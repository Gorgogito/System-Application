using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface ITypeConceptService
{
    Task<IEnumerable<TypeConceptDto>> GetAllAsync();
    Task<TypeConceptDto> GetByCodeAsync(string code);
    Task<TypeConceptDto> CreateAsync(CreateTypeConceptRequest request, string user);
    Task<TypeConceptDto> UpdateAsync(UpdateTypeConceptRequest request);
    Task DeleteAsync(int id);
}
