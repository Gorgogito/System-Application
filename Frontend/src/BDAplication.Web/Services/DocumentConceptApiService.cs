using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class DocumentConceptApiService : ApiService
{
    public DocumentConceptApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<DocumentConceptModel>>?> GetAllAsync() =>
        await GetAsync<List<DocumentConceptModel>>("api/documentconcepts");

    public async Task<ApiResponse<DocumentConceptModel>?> CreateAsync(CreateDocumentConceptRequest req) =>
        await PostAsync<DocumentConceptModel>("api/documentconcepts", req);

    public async Task<ApiResponse<DocumentConceptModel>?> UpdateAsync(int id, UpdateDocumentConceptRequest req) =>
        await PutAsync<DocumentConceptModel>($"api/documentconcepts/{id}", req);

    public async Task<ApiResponse<string>?> DeleteAsync(int id) =>
        await base.DeleteAsync<string>($"api/documentconcepts/{id}");
}
