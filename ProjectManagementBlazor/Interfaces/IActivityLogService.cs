using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;

namespace ProjectManagementBlazor.Interfaces
{
    public interface IActivityLogService
    {
        Task<ApiResponse<PagedResult<ActivityLogResponse>>> GetProjectLogsAsync(GetActivityLogsRequest request);
    }
}