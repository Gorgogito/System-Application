using BDAplication.Domain.Entities.Finance;

namespace BDAplication.Domain.Interfaces.Finance;

public interface IReprocessLogRepository
{
    Task<ReprocessLog> CreateAsync(ReprocessLog log);
    Task<IEnumerable<ReprocessLog>> GetRecentAsync(int limit = 20);
}
