using System.Net.Http.Json;
using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class ApiService
{
    protected readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    protected async Task<ApiResponse<T>?> GetAsync<T>(string url) =>
        await ReadResponseAsync<T>(await _http.GetAsync(url));

    protected async Task<ApiResponse<T>?> PostAsync<T>(string url, object body) =>
        await ReadResponseAsync<T>(await _http.PostAsJsonAsync(url, body));

    protected async Task<ApiResponse<T>?> PutAsync<T>(string url, object body) =>
        await ReadResponseAsync<T>(await _http.PutAsJsonAsync(url, body));

    protected async Task<ApiResponse<T>?> DeleteAsync<T>(string url) =>
        await ReadResponseAsync<T>(await _http.DeleteAsync(url));

    protected async Task<ApiResponse<T>?> PatchAsync<T>(string url, object body) =>
        await ReadResponseAsync<T>(await _http.PatchAsJsonAsync(url, body));

    /// <summary>
    /// Lee el cuerpo de la respuesta como ApiResponse&lt;T&gt;. Algunas respuestas de error
    /// (ej. 401 del middleware de autorización de ASP.NET Core, o un 502/503 de la
    /// infraestructura) no tienen cuerpo JSON — en ese caso no se lanza, se devuelve un
    /// ApiResponse de error genérico con el código de estado.
    /// </summary>
    private static async Task<ApiResponse<T>?> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        }
        catch
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "Sesión expirada o no autorizada. Vuelva a iniciar sesión.",
                    System.Net.HttpStatusCode.Forbidden => "No tiene permisos para esta operación.",
                    _ => $"No se pudo conectar con el servidor (HTTP {(int)response.StatusCode})."
                }
            };
        }
    }
}
