using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class RoleApiService : ApiService
{
    public RoleApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<IEnumerable<RoleModel>>?> GetAllAsync() =>
        await GetAsync<IEnumerable<RoleModel>>("api/master/getrol");

    public async Task<ApiResponse<RoleModel>?> CreateAsync(CreateRoleModel model) =>
        await PostAsync<RoleModel>("api/master/createrol", model);

    public async Task<ApiResponse<RoleModel>?> UpdateAsync(UpdateRoleModel model) =>
        await PutAsync<RoleModel>("api/master/updaterol", model);

    public async Task<ApiResponse<object>?> DeleteAsync(int id) =>
        await DeleteAsync<object>($"api/master/deleterol?id={id}");
}
