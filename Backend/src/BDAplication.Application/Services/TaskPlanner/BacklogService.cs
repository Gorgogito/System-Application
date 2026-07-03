using BDAplication.Application.DTOs.TaskPlanner;
using BDAplication.Application.Interfaces.TaskPlanner;
using BDAplication.Domain.Entities;
using BDAplication.Domain.Interfaces;

namespace BDAplication.Application.Services.TaskPlanner;

public class BacklogService : IBacklogService
{
    private readonly IBacklogRepository _repo;

    public BacklogService(IBacklogRepository repo) => _repo = repo;

    public async Task<BacklogDto> RegisterAsync(BacklogRegisterRequest request, string user)
    {
        var entity = new Backlog
        {
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            UserCreated = user
        };
        var created = await _repo.CreateAsync(entity);
        return ToDto(created);
    }

    public async Task<BacklogListResponse> ListAsync(BacklogListRequest request)
    {
        var items = await _repo.GetAllActiveAsync(request.Search, request.Priority, request.Page, request.PageSize);
        var total = await _repo.CountActiveAsync();
        return new BacklogListResponse(items.Select(ToDto), total, request.Page, request.PageSize);
    }

    private static BacklogDto ToDto(Backlog b) =>
        new(b.Id, b.Name, b.Description, b.Priority, b.Priority.ToString(), b.IsActive, b.UserCreated, b.DateCreated);
}
