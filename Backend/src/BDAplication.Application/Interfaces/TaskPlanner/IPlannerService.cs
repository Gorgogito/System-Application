using BDAplication.Application.DTOs.TaskPlanner;

namespace BDAplication.Application.Interfaces.TaskPlanner;

public interface IPlannerService
{
    Task<PlannerDto> RegisterAsync(PlannerRegisterRequest request, string user);
    Task<PlannerDto> InactivateAsync(PlannerInactiveRequest request);
    Task<IEnumerable<PlannerListByDayDto>> ListByMonthAsync(PlannerListRequest request);
    Task<PlannerDto> MoveAsync(MovePlannerRequest request);
}
