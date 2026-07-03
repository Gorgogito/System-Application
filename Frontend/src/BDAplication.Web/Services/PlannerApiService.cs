using BDAplication.Web.Models;

namespace BDAplication.Web.Services;

public class PlannerApiService : ApiService
{
    public PlannerApiService(HttpClient http) : base(http) { }

    public async Task<ApiResponse<PlannerModel>?> RegisterAsync(PlannerRegisterRequest request) =>
        await PostAsync<PlannerModel>("api/process/planner/PlannerRegister", request);

    public async Task<ApiResponse<PlannerModel>?> InactivateAsync(int plannerId) =>
        await PutAsync<PlannerModel>("api/process/planner/PlannerInactive",
            new PlannerInactiveRequest { PlannerId = plannerId });

    public async Task<ApiResponse<IEnumerable<PlannerListByDayModel>>?> ListByMonthAsync(int month, int year) =>
        await PatchAsync<IEnumerable<PlannerListByDayModel>>("api/process/planner/PlannerList",
            new PlannerListRequest { Month = month, Year = year });

    public async Task<ApiResponse<PlannerModel>?> MoveAsync(MovePlannerRequest request) =>
        await PatchAsync<PlannerModel>("api/process/planner/MovePlanner", request);
}
