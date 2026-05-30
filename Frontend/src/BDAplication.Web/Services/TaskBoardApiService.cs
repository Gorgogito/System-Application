using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class TaskBoardApiService : ApiService
{
    public TaskBoardApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<List<TaskBoardModel>>?> GetTasksAsync(
        TaskBoardStatus? status = null, Priority? priority = null, string? search = null)
    {
        var query = "api/process/taskboard/GetTasks";
        var parts = new List<string>();
        if (status.HasValue)   parts.Add($"status={(int)status.Value}");
        if (priority.HasValue) parts.Add($"priority={(int)priority.Value}");
        if (!string.IsNullOrWhiteSpace(search)) parts.Add($"search={Uri.EscapeDataString(search)}");
        if (parts.Count > 0) query += "?" + string.Join("&", parts);
        return await GetAsync<List<TaskBoardModel>>(query);
    }

    public async Task<ApiResponse<TaskBoardModel>?> CreateAsync(CreateTaskBoardRequest request) =>
        await PostAsync<TaskBoardModel>("api/process/taskboard/CreateTask", request);

    public async Task<ApiResponse<TaskBoardModel>?> UpdateAsync(UpdateTaskBoardRequest request) =>
        await PutAsync<TaskBoardModel>("api/process/taskboard/UpdateTask", request);

    public async Task<ApiResponse<TaskBoardModel>?> MoveAsync(MoveTaskBoardRequest request) =>
        await PatchAsync<TaskBoardModel>("api/process/taskboard/MoveTask", request);

    public async Task<ApiResponse<TaskBoardModel>?> DeleteTaskAsync(int id) =>
        await DeleteAsync<TaskBoardModel>($"api/process/taskboard/DeleteTask?id={id}");

    // Subtareas
    public async Task<ApiResponse<SubTaskModel>?> CreateSubTaskAsync(CreateSubTaskRequest request) =>
        await PostAsync<SubTaskModel>("api/process/taskboard/subtask/Create", request);

    public async Task<ApiResponse<SubTaskModel>?> ToggleSubTaskAsync(int subTaskId) =>
        await PatchAsync<SubTaskModel>("api/process/taskboard/subtask/Toggle", new ToggleSubTaskRequest { SubTaskId = subTaskId });

    public async Task<ApiResponse<object>?> DeleteSubTaskAsync(int id) =>
        await DeleteAsync<object>($"api/process/taskboard/subtask/Delete?id={id}");
}
