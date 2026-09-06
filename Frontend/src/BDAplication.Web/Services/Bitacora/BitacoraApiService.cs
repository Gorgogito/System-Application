using BDAplication.Web.Models;

namespace BDAplication.Web.Services.Bitacora;

public class BitacoraApiService : ApiService
{
    public BitacoraApiService(HttpClient http) : base(http) { }

    public Task<ApiResponse<BitacoraModel>?> GetByFechaAsync(DateTime fecha) =>
        GetAsync<BitacoraModel>($"api/bitacora/{fecha:yyyy-MM-dd}");

    public Task<ApiResponse<List<BitacoraResumenModel>>?> GetResumenAsync(DateTime desde, DateTime hasta, string? texto)
    {
        var url = $"api/bitacora?desde={desde:yyyy-MM-dd}&hasta={hasta:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(texto))
            url += $"&texto={Uri.EscapeDataString(texto)}";
        return GetAsync<List<BitacoraResumenModel>>(url);
    }

    public Task<ApiResponse<BitacoraModel>?> UpdateObservacionAsync(int id, UpdateBitacoraRequest request) =>
        PutAsync<BitacoraModel>($"api/bitacora/{id}", request);

    public Task<ApiResponse<BitacoraActividadModel>?> CreateActividadAsync(CreateActividadRequest request) =>
        PostAsync<BitacoraActividadModel>("api/bitacora/actividad", request);

    public Task<ApiResponse<BitacoraActividadModel>?> UpdateActividadAsync(int id, UpdateActividadRequest request) =>
        PutAsync<BitacoraActividadModel>($"api/bitacora/actividad/{id}", request);

    public Task<ApiResponse<string>?> DeleteActividadAsync(int id) =>
        DeleteAsync<string>($"api/bitacora/actividad/{id}");

    public async Task<ApiResponse<BitacoraEvidenciaModel>?> UploadEvidenciaAsync(
        int actividadId, Stream fileStream, string fileName, string contentType)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync($"api/bitacora/actividad/{actividadId}/evidencia/upload", content);
        return await response.Content.ReadFromJsonAsync<ApiResponse<BitacoraEvidenciaModel>>();
    }

    public Task<ApiResponse<SasUploadResponse>?> GetSasUploadUrlAsync(int actividadId, SasUploadRequest request) =>
        PostAsync<SasUploadResponse>($"api/bitacora/actividad/{actividadId}/evidencia/sas-upload-url", request);

    public Task<ApiResponse<BitacoraEvidenciaModel>?> ConfirmEvidenciaAsync(int actividadId, ConfirmEvidenciaRequest request) =>
        PostAsync<BitacoraEvidenciaModel>($"api/bitacora/actividad/{actividadId}/evidencia/confirm", request);

    public Task<ApiResponse<string>?> GetEvidenciaUrlAsync(int evidenciaId) =>
        GetAsync<string>($"api/bitacora/evidencia/{evidenciaId}/url");

    public Task<ApiResponse<string>?> DeleteEvidenciaAsync(int evidenciaId) =>
        DeleteAsync<string>($"api/bitacora/evidencia/{evidenciaId}");
}
