using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class AttachmentApiService : ApiService
{
    public AttachmentApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<AttachmentModel>>?> GetAllAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "api/attachments"
            : $"api/attachments?search={Uri.EscapeDataString(search)}";
        return await GetAsync<List<AttachmentModel>>(url);
    }

    public async Task<ApiResponse<List<AttachmentModel>>?> GetByEntityAsync(string entityType, int entityId) =>
        await GetAsync<List<AttachmentModel>>($"api/attachments/{entityType}/{entityId}");

    public async Task<ApiResponse<AttachmentModel>?> UploadAsync(
        string entityType, int entityId,
        Stream fileStream, string fileName, string contentType, long fileSize,
        string description, string comment, DateTime documentDate, int? documentConceptId = null,
        decimal? amount = null)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(entityType), "entityType");
        content.Add(new StringContent(entityId.ToString()), "entityId");
        content.Add(new StringContent(description), "description");
        content.Add(new StringContent(comment), "comment");
        content.Add(new StringContent(documentDate.ToString("o")), "documentDate");
        if (documentConceptId.HasValue)
            content.Add(new StringContent(documentConceptId.Value.ToString()), "documentConceptId");
        if (amount.HasValue)
            content.Add(new StringContent(amount.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)), "amount");

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/attachments/upload", content);
        return await response.Content.ReadFromJsonAsync<ApiResponse<AttachmentModel>>();
    }

    public async Task<ApiResponse<AttachmentModel>?> UpdateAsync(int id, UpdateAttachmentRequest request) =>
        await PutAsync<AttachmentModel>($"api/attachments/{id}", request);

    public async Task<ApiResponse<string>?> GetUrlAsync(int id) =>
        await GetAsync<string>($"api/attachments/{id}/url");

    public async Task<ApiResponse<string>?> DeleteAsync(int id) =>
        await base.DeleteAsync<string>($"api/attachments/{id}");
}
