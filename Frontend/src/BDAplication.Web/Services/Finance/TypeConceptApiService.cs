using BDAplication.Web.Models;
using BDAplication.Web.Models.Finance;

namespace BDAplication.Web.Services.Finance;

public class TypeConceptApiService : ApiService
{
    public TypeConceptApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<TypeConceptModel>>?> ListAsync() =>
        await GetAsync<List<TypeConceptModel>>("api/finance/typeconcept/list");

    public async Task<ApiResponse<TypeConceptModel>?> CreateAsync(CreateTypeConceptRequest request) =>
        await PostAsync<TypeConceptModel>("api/finance/typeconcept/create", request);

    public async Task<ApiResponse<TypeConceptModel>?> UpdateAsync(UpdateTypeConceptRequest request) =>
        await PutAsync<TypeConceptModel>("api/finance/typeconcept/update", request);

    public async Task<ApiResponse<object>?> DeleteAsync(int id) =>
        await DeleteAsync<object>($"api/finance/typeconcept/delete?id={id}");
}
