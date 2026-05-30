using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class BacklogApiService : ApiService
{
    public BacklogApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<BacklogListResponse>?> GetListAsync(
        string? search = null, Priority? priority = null, int page = 1, int pageSize = 50)
    {
        var query = $"api/process/backlog/BacklogList?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) query += $"&search={Uri.EscapeDataString(search)}";
        if (priority.HasValue) query += $"&priority={(int)priority.Value}";
        return await GetAsync<BacklogListResponse>(query);
    }

    public async Task<ApiResponse<BacklogModel>?> RegisterAsync(BacklogRegisterRequest request) =>
        await PostAsync<BacklogModel>("api/process/backlog/BacklogRegister", request);
}
