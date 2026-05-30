using System.Net.Http.Json;
using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class ApiService
{
    protected readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    protected async Task<ApiResponse<T>?> GetAsync<T>(string url) =>
        await _http.GetFromJsonAsync<ApiResponse<T>>(url);

    protected async Task<ApiResponse<T>?> PostAsync<T>(string url, object body)
    {
        var response = await _http.PostAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
    }

    protected async Task<ApiResponse<T>?> PutAsync<T>(string url, object body)
    {
        var response = await _http.PutAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
    }

    protected async Task<ApiResponse<T>?> DeleteAsync<T>(string url)
    {
        var response = await _http.DeleteAsync(url);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
    }

    protected async Task<ApiResponse<T>?> PatchAsync<T>(string url, object body)
    {
        var response = await _http.PatchAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
    }
}
