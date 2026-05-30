using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class UserApiService : ApiService
{
    public UserApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<IEnumerable<UserModel>>?> GetAllAsync() =>
        await GetAsync<IEnumerable<UserModel>>("api/master/getuser");

    public async Task<ApiResponse<UserModel>?> CreateAsync(CreateUserModel model) =>
        await PostAsync<UserModel>("api/master/createuser", model);

    public async Task<ApiResponse<UserModel>?> UpdateAsync(UpdateUserModel model) =>
        await PutAsync<UserModel>("api/master/updateuser", model);

    public async Task<ApiResponse<object>?> DeleteAsync(int id) =>
        await DeleteAsync<object>($"api/master/deleteuser?id={id}");
}
