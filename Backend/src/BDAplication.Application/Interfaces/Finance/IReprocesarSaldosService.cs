using BDAplication.Application.DTOs.Finance;

namespace BDAplication.Application.Interfaces.Finance;

public interface IReprocesarSaldosService
{
    Task<ReprocesarResultDto> ExecuteAsync(ReprocesarRequest request, string user);
    Task<IEnumerable<ReprocesarLogDto>> GetHistoryAsync(int limit = 20);
}
