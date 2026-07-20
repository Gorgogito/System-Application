using BDAplication.Web.Models;
using BDAplication.Web.Models.Finance;

namespace BDAplication.Web.Services.Finance;

public class ReprocesarApiService : ApiService
{
    public ReprocesarApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<ReprocesarResultModel>?> ExecuteAsync(ReprocesarRequest request) =>
        await PostAsync<ReprocesarResultModel>("api/finance/reprocess/execute", request);

    public async Task<ApiResponse<List<ReprocesarLogModel>>?> GetHistoryAsync(int limit = 20) =>
        await GetAsync<List<ReprocesarLogModel>>($"api/finance/reprocess/history?limit={limit}");
}
