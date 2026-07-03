using BDAplication.Application.DTOs.Finance;
using BDAplication.Application.Interfaces.Finance;
using BDAplication.Domain.Entities.Finance;
using BDAplication.Domain.Interfaces.Finance;

namespace BDAplication.Application.Services.Finance;

public class TypeConceptService : ITypeConceptService
{
    private readonly ITypeConceptRepository _repo;
    public TypeConceptService(ITypeConceptRepository repo) => _repo = repo;

    public async Task<IEnumerable<TypeConceptDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(ToDto);

    public async Task<TypeConceptDto> GetByCodeAsync(string code)
    {
        var e = await _repo.GetByCodeAsync(code) ?? throw new KeyNotFoundException($"TypeConcept '{code}' not found");
        return ToDto(e);
    }

    public async Task<TypeConceptDto> CreateAsync(CreateTypeConceptRequest request, string user)
    {
        var code = await _repo.GenerateCodeAsync();
        var entity = new TypeConcept
        {
            Code = code,
            Description = request.Description,
            CreatedBy = user
        };
        return ToDto(await _repo.CreateAsync(entity));
    }

    public async Task<TypeConceptDto> UpdateAsync(UpdateTypeConceptRequest request)
    {
        var entity = await _repo.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException($"TypeConcept {request.Id} not found");
        entity.Description = request.Description;
        return ToDto(await _repo.UpdateAsync(entity));
    }

    public async Task DeleteAsync(int id)
    {
        if (await _repo.IsUsedInMovementsAsync(id))
            throw new InvalidOperationException("No se puede eliminar un tipo de concepto que tiene movimientos asociados.");
        await _repo.DeleteAsync(id);
    }

    private static TypeConceptDto ToDto(TypeConcept tc) =>
        new(tc.Id, tc.Code, tc.Description, tc.CreatedDate, tc.CreatedBy);
}
