using BDAplication.Application.DTOs.Attachments;

namespace BDAplication.Application.Interfaces;

public interface IDocumentConceptService
{
    Task<IEnumerable<DocumentConceptDto>> GetAllActiveAsync();
    Task<DocumentConceptDto> CreateAsync(CreateDocumentConceptRequest request, string user);
    Task<DocumentConceptDto> UpdateAsync(UpdateDocumentConceptRequest request);
    Task DeleteAsync(int id);
}
