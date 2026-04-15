using ProjectManagementBlazor.DTO.Common;
using ProjectManagementBlazor.DTO.Requests;
using ProjectManagementBlazor.DTO.Responses;
using ProjectManagementBlazor.Interfaces;
using System.Net.Http.Json;

namespace ProjectManagementBlazor.Services
{
    public class DashboardService : BaseApiService, IDashboardService
    {
        public DashboardService(HttpClient httpClient, IErrorHandlingService errorHandling)
            : base(httpClient, errorHandling)
        {
        }

        public async Task<ApiResponse<PersonalDashboardResponse>> GetPersonalDashboardAsync(DashboardRequest? request = null)
        {
            var queryString = "";
            if (request != null)
            {
                var paramsList = new List<string>();
                if (request.Date.HasValue)
                    paramsList.Add($"date={request.Date.Value:yyyy-MM-dd}");
                if (request.ProjectId.HasValue)
                    paramsList.Add($"projectId={request.ProjectId}");
                if (request.IncludeAllProjects)
                    paramsList.Add("includeAllProjects=true");

                if (paramsList.Any())
                    queryString = "?" + string.Join("&", paramsList);
            }

            return await SendRequestAsync<PersonalDashboardResponse>(
                () => _httpClient.GetAsync($"api/dashboard/my-day{queryString}"),
                "Не удалось загрузить дашборд");
        }

        public async Task<ApiResponse<DailyScrumResponse>> GetDailyScrumViewAsync(Guid projectId, Guid? sprintId = null)
        {
            var url = $"api/dashboard/daily-scrum?projectId={projectId}";
            if (sprintId.HasValue)
                url += $"&sprintId={sprintId}";

            return await SendRequestAsync<DailyScrumResponse>(
                () => _httpClient.GetAsync(url),
                "Не удалось загрузить Daily Scrum");
        }

        public async Task<ApiResponse> UpdateDailyTasksAsync(UpdateDailyTasksRequest request)
        {
            return await SendRequestAsync(
                () => _httpClient.PostAsJsonAsync("api/dashboard/my-day", request),
                "Не удалось сохранить ежедневные задачи");
        }
    }
}