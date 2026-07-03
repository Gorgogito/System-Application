using BDAplication.Application.DTOs.Attachments;
using BDAplication.Application.Interfaces;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services;

public class DocumentConceptService : IDocumentConceptService
{
    private readonly IDocumentConceptRepository _repo;
    public DocumentConceptService(IDocumentConceptRepository repo) => _repo = repo;

    public async Task<IEnumerable<DocumentConceptDto>> GetAllActiveAsync()
    {
        var list = await _repo.GetAllActiveAsync();
        return list.Select(ToDto);
    }

    public async Task<DocumentConceptDto> CreateAsync(CreateDocumentConceptRequest request, string user)
    {
        var code = await _repo.GenerateCodeAsync();
        var entity = new DocumentConcept
        {
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            CreatedBy = user,
            CreatedDate = DateTime.UtcNow
        };
        var created = await _repo.CreateAsync(entity);
        return ToDto(created);
    }

    public async Task<DocumentConceptDto> UpdateAsync(UpdateDocumentConceptRequest request)
    {
        var entity = await _repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Concepto {request.Id} no encontrado");
        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        var updated = await _repo.UpdateAsync(entity);
        return ToDto(updated);
    }

    public async Task DeleteAsync(int id)
    {
        if (await _repo.IsUsedAsync(id))
            throw new InvalidOperationException("No se puede eliminar: el concepto está en uso por archivos adjuntos.");
        await _repo.DeleteAsync(id);
    }

    private static DocumentConceptDto ToDto(DocumentConcept c) =>
        new(c.Id, c.Code, c.Name, c.Description, c.IsActive, c.CreatedBy, c.CreatedDate);
}
