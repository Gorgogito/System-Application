using BDAplication.Domain.Entities;

namespace BDAplication.Domain.Interfaces;

public interface IPlannerRepository
{
    Task<IEnumerable<Planner>> GetByMonthYearAsync(int month, int year);
    Task<Planner?> GetByIdAsync(int id);
    Task<Planner> CreateAsync(Planner planner);
    Task<Planner> UpdateAsync(Planner planner);
}
