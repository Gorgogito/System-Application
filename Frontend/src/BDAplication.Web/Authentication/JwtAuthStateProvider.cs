using System.Security.Claims;
using System.Text.Json;
using BDAplication.Web.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BDAplication.Web.Authentication;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _session;
    private readonly HttpClient _http;

    public JwtAuthStateProvider(ProtectedSessionStorage session, HttpClient http)
    {
        _session = session;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var tokenResult = await _session.GetAsync<string>("jwt_token");
            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
                return Anonymous();

            var token = tokenResult.Value;
            var claims = ParseClaimsFromJwt(token);
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null)
            {
                var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                if (exp < DateTimeOffset.UtcNow)
                {
                    await LogoutAsync();
                    return Anonymous();
                }
            }

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return Anonymous();
        }
    }

    public async Task LoginAsync(LoginResponse response)
    {
        await _session.SetAsync("jwt_token", response.Token);
        await _session.SetAsync("current_user", JsonSerializer.Serialize(response.User));
        await _session.SetAsync("current_role", JsonSerializer.Serialize(response.Role));

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Token);

        var claims = ParseClaimsFromJwt(response.Token);
        var identity = new ClaimsIdentity(claims, "jwt");
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task LogoutAsync()
    {
        await _session.DeleteAsync("jwt_token");
        await _session.DeleteAsync("current_user");
        await _session.DeleteAsync("current_role");
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
    }

    public async Task<UserModel?> GetCurrentUserAsync()
    {
        try
        {
            var result = await _session.GetAsync<string>("current_user");
            return result.Success && !string.IsNullOrEmpty(result.Value)
                ? JsonSerializer.Deserialize<UserModel>(result.Value)
                : null;
        }
        catch { return null; }
    }

    public async Task<RoleModel?> GetCurrentRoleAsync()
    {
        try
        {
            var result = await _session.GetAsync<string>("current_role");
            return result.Success && !string.IsNullOrEmpty(result.Value)
                ? JsonSerializer.Deserialize<RoleModel>(result.Value)
                : null;
        }
        catch { return null; }
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes)!;

        return keyValuePairs.Select(kvp =>
            new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
