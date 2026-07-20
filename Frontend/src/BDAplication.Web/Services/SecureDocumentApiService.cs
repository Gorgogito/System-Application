using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class SecureDocumentApiService : ApiService
{
    public SecureDocumentApiService(HttpClient http) : base(http) { }

    public Task<ApiResponse<IEnumerable<SecureDocumentListItemModel>>?> GetAllAsync() =>
        GetAsync<IEnumerable<SecureDocumentListItemModel>>("api/secure-docs");

    public Task<ApiResponse<SecureDocumentModel>?> GetByIdAsync(int id) =>
        GetAsync<SecureDocumentModel>($"api/secure-docs/{id}");

    public Task<ApiResponse<SecureDocumentModel>?> CreateAsync(CreateSecureDocumentRequest request) =>
        PostAsync<SecureDocumentModel>("api/secure-docs", request);

    public Task<ApiResponse<SecureDocumentModel>?> AddVersionAsync(int id, AddVersionRequest request) =>
        PostAsync<SecureDocumentModel>($"api/secure-docs/{id}/versions", request);

    public Task<ApiResponse<DecryptedContentModel>?> DecryptAsync(int id, DecryptRequest request) =>
        PostAsync<DecryptedContentModel>($"api/secure-docs/{id}/decrypt", request);

    public Task<ApiResponse<bool>?> DeleteAsync(int id, DecryptRequest credentials) =>
        PostAsync<bool>($"api/secure-docs/{id}/delete", credentials);
}
