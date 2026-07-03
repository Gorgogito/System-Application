namespace BDAplication.Web.Services.Finance;

public class AccountStatementApiService : ApiService
{
    public AccountStatementApiService(HttpClient http) : base(http) { }

    public async Task<byte[]?> GetExcelAsync(int accountId, DateTime startDate, DateTime endDate)
    {
        string url = $"api/finance/statement/excel?accountId={accountId}" +
                     $"&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]?> GetPdfAsync(int accountId, DateTime startDate, DateTime endDate)
    {
        string url = $"api/finance/statement/pdf?accountId={accountId}" +
                     $"&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadAsByteArrayAsync();
    }
}
