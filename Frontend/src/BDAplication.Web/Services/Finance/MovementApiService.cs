using BDAplication.Web.Models;
using BDAplication.Web.Models.Finance;

namespace BDAplication.Web.Services.Finance;

public class MovementApiService : ApiService
{
    public MovementApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<MovementModel>>?> ListByAccountAsync(int accountId) =>
        await GetAsync<List<MovementModel>>($"api/finance/movement/listbyaccount?accountId={accountId}");

    public async Task<ApiResponse<MovementModel>?> CreateAsync(CreateMovementRequest request) =>
        await PostAsync<MovementModel>("api/finance/movement/create", request);

    public async Task<ApiResponse<MovementModel>?> UpdateAsync(UpdateMovementRequest request) =>
        await PutAsync<MovementModel>("api/finance/movement/update", request);

    public async Task<ApiResponse<object>?> DeleteAsync(int id) =>
        await DeleteAsync<object>($"api/finance/movement/delete?id={id}");

    public async Task<ApiResponse<object>?> TransferAsync(CreateTransferRequest request) =>
        await PostAsync<object>("api/finance/movement/transfer", request);
}
