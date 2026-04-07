using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;


namespace ProjectManagementBlazor.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<PersonalDashboardResponse>> GetPersonalDashboardAsync(DashboardRequest? request = null);
        Task<ApiResponse<DailyScrumResponse>> GetDailyScrumViewAsync(Guid projectId, Guid? sprintId = null);
        Task<ApiResponse> UpdateDailyTasksAsync(UpdateDailyTasksRequest request);
    }
}