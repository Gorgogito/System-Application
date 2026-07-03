using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class AuthApiService : ApiService
{
    public AuthApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<LoginResponse>?> LoginAsync(LoginRequest request) =>
        await PostAsync<LoginResponse>("api/master/authentication", request);
}
