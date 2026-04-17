// Services/ActivityLogService.cs (клиент)
using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;

namespace ProjectManagementBlazor.Services
{
    public class ActivityLogService : BaseApiService, IActivityLogService
    {
        public ActivityLogService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<PagedResult<ActivityLogResponse>>> GetProjectLogsAsync(GetActivityLogsRequest request)
        {
            var queryString = $"?projectId={request.ProjectId}&pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            if (request.UserId.HasValue)
                queryString += $"&userId={request.UserId}";
            if (!string.IsNullOrEmpty(request.ActionType))
                queryString += $"&actionType={Uri.EscapeDataString(request.ActionType)}";
            if (request.DateFrom.HasValue)
                queryString += $"&dateFrom={request.DateFrom.Value:yyyy-MM-dd}";
            if (request.DateTo.HasValue)
                queryString += $"&dateTo={request.DateTo.Value:yyyy-MM-dd}";

            return await SendRequestAsync<PagedResult<ActivityLogResponse>>(
                () => _httpClient.GetAsync($"api/activitylogs{queryString}"),
                "Не удалось загрузить логи активности");
        }
    }
}