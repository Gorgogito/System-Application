using BDAplication.Application.DTOs.TaskPlanner;

namespace BDAplication.Application.Interfaces.TaskPlanner;

public interface IBacklogService
{
    Task<BacklogDto> RegisterAsync(BacklogRegisterRequest request, string user);
    Task<BacklogListResponse> ListAsync(BacklogListRequest request);
}
