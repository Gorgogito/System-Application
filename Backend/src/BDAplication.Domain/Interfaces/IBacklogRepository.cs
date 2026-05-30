using BDAplication.Domain.Entities;
using BDAplication.Domain.Enums;

namespace BDAplication.Domain.Interfaces;

public interface IBacklogRepository
{
    Task<IEnumerable<Backlog>> GetAllActiveAsync(string? search = null, Priority? priority = null, int page = 1, int pageSize = 50);
    Task<Backlog?> GetByIdAsync(int id);
    Task<Backlog> CreateAsync(Backlog backlog);
    Task<Backlog> UpdateAsync(Backlog backlog);
    Task<int> CountActiveAsync();
}
