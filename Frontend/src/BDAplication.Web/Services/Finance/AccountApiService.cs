using BDAplication.Web.Models;
using BDAplication.Web.Models.Finance;

namespace BDAplication.Web.Services.Finance;

public class AccountApiService : ApiService
{
    public AccountApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<AccountModel>>?> ListAsync() =>
        await GetAsync<List<AccountModel>>("api/finance/account/list");

    public async Task<ApiResponse<AccountModel>?> GetAsync(string code) =>
        await GetAsync<AccountModel>($"api/finance/account/get?code={Uri.EscapeDataString(code)}");

    public async Task<ApiResponse<AccountModel>?> CreateAsync(CreateAccountRequest request) =>
        await PostAsync<AccountModel>("api/finance/account/create", request);

    public async Task<ApiResponse<AccountModel>?> UpdateAsync(UpdateAccountRequest request) =>
        await PutAsync<AccountModel>("api/finance/account/update", request);

    public async Task<ApiResponse<object>?> DeleteAsync(int id) =>
        await DeleteAsync<object>($"api/finance/account/delete?id={id}");
}
